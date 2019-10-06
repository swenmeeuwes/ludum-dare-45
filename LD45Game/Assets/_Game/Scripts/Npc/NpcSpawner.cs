using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour {
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _npcTarget;
    [SerializeField] private Transform _npcLeave;

    [SerializeField] private Npc _npcPrefab;

    public static NpcSpawner Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public Npc Spawn(NpcModel npcModel, bool activateInstantly = true) {
        var npc = Instantiate(_npcPrefab);
        npc.transform.position = _spawnPoint.transform.position;
        npc.Model = npcModel;
        npc.ShopWaypoint = _npcTarget;
        npc.LeaveWaypoint = _npcLeave;

        if (activateInstantly) {
            npc.Activate();
        }

        return npc;
    }
}