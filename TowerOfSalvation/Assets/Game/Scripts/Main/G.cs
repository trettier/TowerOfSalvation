using System;
using System.Collections.Generic;
using System.Linq;
using TowerOfSalvation.Audio;
using TowerOfSalvation.Effects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class G : Singleton<G>
{
    public GameData gameData;
    public ZonesSettings zoneSettings;
    public CharactersLevelsSettings charactersLevelsSettings;
    public ItemsLevelsSettings itemsLevelsSettings;
    public SidesSettings sidesSettings;
    public EffectsSettings effectsSettings;
    public Damage damage;
    public EntityFactory entityFactory;
    public UIFactory uiFactory;
    public CameraManager cameraManager;

    public AudioManager audioManager;
    public VisualEffectsFactory visualEffectsFactory;

    private GameState _currentState;

    public ResourceService resourceService;

    private void Start()
    {
        resourceService.Initialize();
        resourceService.ResetPlayer();
        resourceService.Add(ResourceType.Victual, 2);

        ChangeState(new LobbyState());
    }

    private void Update()
    {
        _currentState.Update();
    }

    public void ChangeState(GameState newState)
    {
        if (_currentState != null)
            _currentState.Exit();
        _currentState = newState;
        _currentState.Enter(this);
    }

    public void EnterBattle()
    {
        ChangeState(new CombatState());
    }

    public void EnterLobby()
    {
        ChangeState(new LobbyState());
    }

    private void OnDestroy()
    {
        if (_currentState != null)
            _currentState.Exit();
    }
}

public abstract class GameState
{
    public G G;
    public virtual void Enter(G G) 
    {
        this.G = G;
    }
    public virtual void Update() { }
    public virtual void Exit() { }
}

public class LobbyState : GameState
{
    private LobbySetup levelSetup;
    public override void Enter(G G)
    {
        base.Enter(G);
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene("lobby", LoadSceneMode.Single);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        levelSetup = (LobbySetup)GameObject.FindObjectsByType(typeof(LobbySetup), FindObjectsSortMode.None)[0];

        LoadZones();
        InitializeCamera();

        levelSetup.teleportationZone.TeleportCharacters();
    }

    public void LoadZones()
    {                
        var combatZone = G.zoneSettings.zones.First(zone => zone.name == "combat");
        levelSetup.combatZone.Initialize(combatZone);

        var teleportationZone = G.zoneSettings.zones.First(zone => zone.name == "teleportation");
        levelSetup.teleportationZone.Initialize(teleportationZone);
    }

    public void InitializeCamera()
    {
        G.cameraManager.BoundsFromTilemap(levelSetup.floor);
        G.cameraManager.Initialize(levelSetup.teleportationZone);
    }

    public override void Exit()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}


public class CombatState : GameState
{
    private bool isInitilized = false;
    private CombatZone combatZone;
    private Level_1 levelSetup;

    public override void Enter(G G)
    {
        base.Enter(G);
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene("lawn", LoadSceneMode.Single);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadZones();
        InitializeCamera();

        combatZone.ChangeState(new ZoneCombat());

        G.cameraManager.SetFollowState();

        isInitilized = true;
    }


    public override void Update()
    {
        if (!isInitilized)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
            combatZone.SetTarget((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void LoadZones()
    {
        levelSetup = (Level_1)GameObject.FindObjectsByType(typeof(LevelSetup), FindObjectsSortMode.None)[0];
        var combatZoneData = G.zoneSettings.zones.First(zone => zone.name == "combat");
        combatZone = levelSetup.combatZone;
        combatZone.Initialize(combatZoneData);
    }

    public void InitializeCamera()
    {
        G.cameraManager.BoundsFromTilemap(levelSetup.floor);
        G.cameraManager.ChangeTarget(levelSetup.combatZone);
    }

    public override void Exit()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}