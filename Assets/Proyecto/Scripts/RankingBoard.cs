using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using System.IO;

[Serializable]
public class RankingData
{
    public List<RankingEntry> entries = new List<RankingEntry>();
    public int nextId = 1;
}

[Serializable]
public class RankingEntry
{
    public int id;              // único y fijo: no cambia aunque el ranking se reordene
    public string playerName;
    public int roundNumber;
    public int[] throws;        // los 3 tiros de la ronda
    public int total;           // suma de los tiros
}

public class RankingBoard : MonoBehaviour
{
    public static RankingBoard Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private TMP_Text rankingText;

    [Header("Configuración")]
    [SerializeField, Min(1)] private int maxVisibleEntries = 8;
    [SerializeField, Min(3)] private int maxNameLength = 10;

    [Header("Colores")]
    [SerializeField] private Color headerColor = new Color(0.7f, 0.7f, 0.65f);
    [SerializeField] private Color normalColor = new Color(0.95f, 0.95f, 0.9f);
    [SerializeField] private Color highlightColor = new Color(1f, 0.85f, 0.25f);

    private readonly List<RankingEntry> entries = new List<RankingEntry>();
    private readonly StringBuilder sb = new StringBuilder();
    private int nextId = 1;
    private int lastEntryId = -1;
    private string SavePath =>
    Path.Combine(Application.persistentDataPath, "ranking.json");

    public IReadOnlyList<RankingEntry> Entries => entries;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Load();
        Refresh();
    }

    // Agrega una ronda completada. Devuelve el id de la entrada.
    public int AddEntry(string playerName, int roundNumber, int[] throws)
    {
        int[] copy = throws != null ? (int[])throws.Clone() : new int[0];
        int total = 0;
        foreach (int t in copy) total += t;

        var entry = new RankingEntry
        {
            id = nextId++,
            playerName = string.IsNullOrEmpty(playerName) ? "?" : playerName,
            roundNumber = roundNumber,
            throws = copy,
            total = total
        };

        entries.Add(entry);
        lastEntryId = entry.id;
        Save();
        Refresh();
        return entry.id;
    }

    // Quita una entrada por su id.
    public bool RemoveEntry(int id)
    {
        int removed = entries.RemoveAll(e => e.id == id);
        if (removed == 0) return false;

        if (id == lastEntryId) lastEntryId = -1; // ya no hay "última ronda" que resaltar
        Save();
        Refresh();
        return true;
    }

    // Quita la entrada agregada más recientemente.
    public bool RemoveLastEntry()
    {
        if (entries.Count == 0) return false;
        return RemoveEntry(entries[entries.Count - 1].id);
    }

    // Borra todo (por ejemplo, al empezar una partida nueva).
    public void Clear()
    {
        entries.Clear();
        nextId = 1;
        lastEntryId = -1;
        Save();
        Refresh();
    }

    private void Refresh()
    {
        if (rankingText == null) return;

        // Copia ordenada: mayor puntaje primero; si empatan, gana la más antigua
        var ranked = new List<RankingEntry>(entries);
        ranked.Sort((a, b) =>
        {
            int cmp = b.total.CompareTo(a.total);
            return cmp != 0 ? cmp : a.id.CompareTo(b.id);
        });

        sb.Clear();
        AppendHeader();

        int visible = Mathf.Min(ranked.Count, maxVisibleEntries);
        bool lastShown = false;

        for (int i = 0; i < visible; i++)
        {
            AppendRow(i + 1, ranked[i]);
            if (ranked[i].id == lastEntryId) lastShown = true;
        }

        // Si la última ronda no entró en el top, se muestra aparte con su posición real
        if (!lastShown && lastEntryId != -1)
        {
            int idx = ranked.FindIndex(e => e.id == lastEntryId);
            if (idx >= 0)
            {
                sb.AppendLine("...");
                AppendRow(idx + 1, ranked[idx]);
            }
        }

        rankingText.text = sb.ToString();
    }

    private void AppendHeader()
    {
        string hex = ColorUtility.ToHtmlStringRGB(headerColor);
        sb.Append("<color=#").Append(hex).Append('>')
          .Append("POS<pos=10%>JUGADOR<pos=40%>RONDA<pos=55%>PTS<pos=70%>TIROS")
          .AppendLine("</color>");
    }

    private void AppendRow(int position, RankingEntry e)
    {
        bool isLast = e.id == lastEntryId;
        string hex = ColorUtility.ToHtmlStringRGB(isLast ? highlightColor : normalColor);

        sb.Append("<color=#").Append(hex).Append('>');
        if (isLast) sb.Append("<b>");

        sb.Append(position).Append('°');
        sb.Append("<pos=10%>").Append(Truncate(e.playerName, maxNameLength));
        sb.Append("<pos=40%>#").Append(e.roundNumber);
        sb.Append("<pos=55%>").Append(e.total);
        sb.Append("<pos=70%>(").Append(string.Join(", ", e.throws)).Append(')');

        if (isLast) sb.Append("</b>");
        sb.AppendLine("</color>");
    }

    private static string Truncate(string s, int max)
    {
        return s.Length <= max ? s : s.Substring(0, max);
    }

    // --- Pruebas rápidas desde el Inspector (menú ⋮ del componente, en Play) ---
    [ContextMenu("Test/Agregar entrada aleatoria")]
    private void TestAdd()
    {
        int[] t =
        {
            UnityEngine.Random.Range(0, 61),
            UnityEngine.Random.Range(0, 61),
            UnityEngine.Random.Range(0, 61)
        };
        AddEntry("Jugador 1", entries.Count + 1, t);
    }

    [ContextMenu("Test/Quitar última")]
    private void TestRemoveLast() => RemoveLastEntry();

    private void Save()
    {
        RankingData data = new RankingData
        {
            entries = entries,
            nextId = nextId
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    private void Load()
    {
        if (!File.Exists(SavePath))
            return;

        string json = File.ReadAllText(SavePath);
        RankingData data = JsonUtility.FromJson<RankingData>(json);

        if (data == null)
            return;

        entries.Clear();

        if (data.entries != null)
            entries.AddRange(data.entries);

        nextId = Mathf.Max(1, data.nextId);

        // No se conserva el resaltado de la última entrada entre ejecuciones.
        lastEntryId = -1;
    }
    public void DeleteHistory()
    {
        entries.Clear();
        nextId = 1;
        lastEntryId = -1;

        if (File.Exists(SavePath))
            File.Delete(SavePath);

        Refresh();

        Debug.Log("Historial del ranking eliminado.");
    }

    [ContextMenu("Ranking/Borrar historial")]
    private void TestDeleteHistory()
    {
        DeleteHistory();
    }
}