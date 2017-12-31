using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get { return instance; } }
    private static GameController instance = null;

    public NextTurnPanel nextTurnPanel;
    public Team ActiveTeam { get { return activeTeam; } }
    public Team[] Teams { get { return teams.ToArray(); } }

    public GameObject selectionMarkerPrefab;

    [HideInInspector]
    public SelectionMarker marker;

    public Color[] teamColors = { Color.red, Color.blue, Color.green, Color.yellow };

    private List<Team> teams = new List<Team>();
    private Team activeTeam = null;

    private GameController() { }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            DestroyImmediate(gameObject);

        if (selectionMarkerPrefab)
        {
            marker = Instantiate(selectionMarkerPrefab).GetComponent<SelectionMarker>();
        }
        else
            Debug.LogError("Selection marker prefab missing!");

        if (!nextTurnPanel)
            Debug.LogWarning("No 'Next turn' panel attached!");

        CreateTeams();
        if (teams.Count <= 1)
        {
            Debug.LogError("There's less than 2 teams in the game!");
        }

        TurnBegin();
    }

    /// <summary>
    /// Method creates Team components based on units' team numbers.
    /// </summary>
    private void CreateTeams()
    {
        // count teams
        SelectableObject[] objects = FindObjectsOfType<SelectableObject>();
        List<int> teamNrs = new List<int>();
        foreach (SelectableObject selectable in objects)
        {
            if (!teamNrs.Contains(selectable.team))
            {
                teamNrs.Add(selectable.team);
            }
        }
        teamNrs.Sort();

        // create team components
        foreach (int nr in teamNrs)
        {
            if (nr == 0)
                continue;

            Team team = gameObject.AddComponent<Team>();
            team.nr = nr;
            team.color = teamColors[(nr - 1) % teamColors.Length];
            teams.Add(team);

            // assign objects to teams
            SelectableObject[] members = objects.Select(n => n).Where(b => b.team == nr).ToArray();
            Debug.Log("Assigning " + members.Length + " member(s) to team nr " + nr);
            foreach (SelectableObject member in members)
            {
                member.AssignToTeam(team);
                teams[nr - 1].Troops.Add(member);
            }
        }
    }

    public void FinishTurn()
    {
        TurnBegin();
    }

    private void TurnBegin()
    {
        // assign next team
        int nextTeamIdx = (teams.IndexOf(activeTeam) + 1) % teams.Count;
        activeTeam = teams[nextTeamIdx];
        nextTurnPanel.Show(activeTeam, 3);
        foreach (Team team in teams)
        {
            team.RefreshTroops();
        }
    }
}
