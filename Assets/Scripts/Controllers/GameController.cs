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
    /// <summary>Currently active team.</summary>
    public Team ActiveTeam { get { return activeTeam; } }
    /// <summary>Array of all teams.</summary>
    public Team[] Teams { get { return teams.ToArray(); } }
    public Team Player { get { return player; } }
    public Lobby Lobby { get { return lobby; } }
    public GameObject selectionMarkerPrefab;
    public Button endTurnButton;

    [HideInInspector]
    public SelectionMarker marker;

    public Color[] teamColors = { Color.red, Color.blue, Color.green, Color.yellow };

    private List<Team> teams = new List<Team>();
    private Team activeTeam = null;
    private Lobby lobby;
    private Team player;

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

        lobby = FindObjectOfType<Lobby>();
        if (!lobby)
            Debug.LogError("Can't find lobby gameobject!");

        player = GameObject.FindWithTag("Player").GetComponent<Team>();

        CreateTeams();
        // if (teams.Count <= 1)
        // {
        //     Debug.LogError("There's less than 2 teams in the game!");
        // }

        activeTeam = teams[0];
        TurnBegin();
    }

    /// <summary>Method creates Team components based on units' team numbers.</summary>
    private void CreateTeams()
    {
        Team[] t = FindObjectsOfType<Team>();
        Team[] toAdd = new Team[t.Length];
        if (t.Length > 1)
        {
            for (int i = 0; i < t.Length; i++)
            {
                toAdd[t[i].nr - 1] = t[i];
            }
            teams = toAdd.ToList();
        }
        else
        {
            // reactivate units
            for (int i = 1; i <= 4; i++)
            {
                GameObject teamObject = GameObject.Find("Team" + i);
                if (teamObject)
                {
                    List<Transform> children = new List<Transform>();
                    foreach (Transform child in teamObject.transform)
                    {
                        children.Add(child);
                    }
                    foreach (Transform tr in children)
                        tr.gameObject.SetActive(true);
                }
            }

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
    }

    /// <summary>Method checks if unit's team has any troops left. If not the other team wins.</summary>
    public void UnitDied(SelectableObject obj)
    {
        Team unitTeam = teams[obj.team - 1];
        unitTeam.Troops.Remove(obj);
        if (!unitTeam.HasUnits())
        {
            GameOver(teams[obj.team % teams.Count]);
        }
    }

    private void GameOver(Team winnerTeam)
    {
        nextTurnPanel.Show("Team " + winnerTeam.nr + " wins!", winnerTeam, 5, () => { LevelLoader.Instance.LoadMainMenu(); });
        enabled = false;
    }

    public void SendEndTurnRequest()
    {
        if (lobby.Online)
            lobby.SendMessage("ENDOFTURN", lobby.RoomPort);
        else
            TurnEndedButtonPressed();
    }

    public void TurnEndedButtonPressed()
    {
        // assign next team
        int nextTeamIdx = (teams.IndexOf(activeTeam) + 1) % teams.Count;
        activeTeam = teams[nextTeamIdx];

        TurnBegin();
    }

    /// <summary>Initiate new turn of next team.</summary>
    private void TurnBegin()
    {
        if (activeTeam)
        {
            foreach (SelectableObject obj in activeTeam.Troops)
            {
                obj.FinishMove();
            }
            
            if (lobby.Online)
            {
                if (activeTeam.nr == player.nr)
                    endTurnButton.interactable = true;
                else
                    endTurnButton.interactable = false;
            }            
        }

        nextTurnPanel.Show(activeTeam, 1.5f);
        foreach (Team team in teams)
        {
            team.RefreshTroops();
        }
    }
}
