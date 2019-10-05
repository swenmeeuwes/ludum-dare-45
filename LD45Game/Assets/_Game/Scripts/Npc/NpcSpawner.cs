﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour {
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _npcTarget;
    [SerializeField] private Transform _npcLeave;

    [SerializeField] private GameObject _npcPrefab;

    private void Start() {
        Spawn();
    }

    private void Spawn() {
        var npc = Instantiate(_npcPrefab);
        npc.transform.position = _spawnPoint.transform.position;

        DOTween.Sequence()
            .Append(npc.transform.DOMove(_npcTarget.position, 2.5f))
            .AppendInterval(2f)
            .Append(npc.transform.DOMove(_npcLeave.position, 2.5f));
    }
}