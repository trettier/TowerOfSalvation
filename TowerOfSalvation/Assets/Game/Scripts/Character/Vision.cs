using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public Observation observation;

    public event Action enemyDetection;
    public event Action enemyLoseSight;

    public bool isInitialized = false;

    private Side mySide;

    private void Awake()
    {
        observation = new Observation();
    }

    public void Initialize(Side mySide)
    {
        this.mySide = mySide;
        isInitialized = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInitialized)
            return;

        var view = collision.gameObject.GetComponent<CharacterView>();
        if (view == null)
            return;

        if (G.instance.sidesSettings.IsEnemy(mySide, view.side))
        {
            observation.enemies.Add(view);
            view.Death += OnEnemyDeath;
            if (observation.enemies.Count == 1)
                enemyDetection?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isInitialized)
            return;

        var view = collision.gameObject.GetComponent<CharacterView>();
        if (view == null || G.instance == null)
            return;

        if (G.instance.sidesSettings.IsEnemy(mySide, view.side))
        {
            view.Death -= OnEnemyDeath;
            observation.enemies.Remove(view);
            if (observation.enemies.Count == 0)
                enemyLoseSight?.Invoke();
        }
    }

    private void OnEnemyDeath(CharacterView view)
    {
        view.Death -= OnEnemyDeath;
        observation.enemies.Remove(view);
        if (observation.enemies.Count == 0)
            enemyLoseSight?.Invoke();
    }

    public CharacterView ClosestEnemy()
    {
        if (observation.enemies.Count == 0)
            return null;

        CharacterView closestEnemy = observation.enemies[0];
        for (int i = 0; i < observation.enemies.Count; i++)
        {
            if ((closestEnemy.transform.position - transform.position).magnitude > (observation.enemies[i].transform.position - transform.position).magnitude)
                closestEnemy = observation.enemies[i];
        }
        return closestEnemy;
    }
}

public class Observation
{
    public Side mySide;

    public List<CharacterView> enemies;
    public List<CharacterView> allies;

    public Observation()
    {
        enemies = new List<CharacterView>();
        allies = new List<CharacterView>();
    }
}