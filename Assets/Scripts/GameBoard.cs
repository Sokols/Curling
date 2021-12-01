using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameBoard : MonoBehaviour
{
    // ile bloków w sumie może się zrespić podczas 1 'endu' (po 3 na każdy zespół)
    private static int MAX_BLOCKS_SPAWNED = 6;
    // na ile 'endów' składa się gra
    private static int ENDS_TOTAL = 2;

    public GameObject SpawnPoint;
    public GameObject House;
    public GameObject IcePlane;
    public GameObject Stone;
    public TMP_Text Player1Score;
    public TMP_Text Player2Score;
    public TMP_Text Result;

    private ScoreModel _score = new ScoreModel(0, 0);
    private SpawnPoint _spawnPoint;
    private House _house;
    private IcePlane _icePlane;
    private IList<GameObject> _redBlocksInGame = new List<GameObject>();
    private IList<GameObject> _blueBlocksInGame = new List<GameObject>();
    private GameObject _currentBlock;
    private Team _currentTeam = Team.RED;
    private int _currentEnd = 1;
    private bool _currentBlockMoving;
    private Vector2 _lastCurrentBlockPosition;
    
    void Start()
    {
        _spawnPoint = SpawnPoint.GetComponent<SpawnPoint>();
        _house = House.GetComponent<House>();
        _icePlane = IcePlane.GetComponent<IcePlane>();
        spawnBlock();
    }

    void Update()
    {
        observeTrackedBlock();
    }

    private void onBlockSpawned(GameObject block) {
        if(_currentTeam == Team.RED) {
            _redBlocksInGame.Add(block);
        } else {
            _blueBlocksInGame.Add(block);
        }
        _currentBlock = block;
        _lastCurrentBlockPosition = _currentBlock.transform.localPosition;
    }

    private void onTurnEnded() {
        _currentBlockMoving = false;
        _currentBlock = null;
        if (getBlocksInGameCount() == MAX_BLOCKS_SPAWNED) {
            onEndFinished();
        } else {
            switchTeam();
            spawnBlock();
        }
    }

    private void onEndFinished() {
        var endScore = calculateEndsScore();
        displayScore(endScore);
        if(_currentEnd == ENDS_TOTAL) {
            onGameEnded();
        } else {
            _currentEnd++;
            clearBoard();
            switchTeam();
            spawnBlock();
        }
    }

    private void onGameEnded() {
        clearBoard();
        if (_score.redPoints > _score.bluePoints) {
            Result.SetText("Red Player won!");
        } else if (_score.redPoints < _score.bluePoints) {
            Result.SetText("Blue Player won!");
        } else {
            Result.SetText("Draw!");
        }
        Result.ForceMeshUpdate(true);
    }

    private void spawnBlock() {
        var newBlock = Instantiate(Stone, SpawnPoint.transform.position, Quaternion.identity);
        newBlock.GetComponent<Stone>().SetStoneColor(_currentTeam);
        newBlock.transform.parent = this.gameObject.transform;
        onBlockSpawned(newBlock);
    }

    private ScoreModel calculateEndsScore() {
        if(_house.isAnyBlockInHouse(_redBlocksInGame, _blueBlocksInGame)) {
            return _house.calculateEndsScore(_redBlocksInGame, _blueBlocksInGame);
        } else {
            return new ScoreModel(0,0);
        }
    }

    private void clearBoard() {
        foreach (var item in _redBlocksInGame)
        {
            Destroy(item);
        }
        _redBlocksInGame.Clear();
        foreach (var item in _blueBlocksInGame)
        {
            Destroy(item);
        }
        _blueBlocksInGame.Clear();
    }

    private void observeTrackedBlock() {
        if(_currentBlock != null) {
            var currentBlockPosition = new Vector2(_currentBlock.transform.localPosition.x, _currentBlock.transform.localPosition.z);
            if(_currentBlockMoving) {
                if(currentBlockPosition == _lastCurrentBlockPosition) {
                    onTurnEnded();
                } else { 
                    _lastCurrentBlockPosition = currentBlockPosition;
                }
            } else if(currentBlockPosition != _lastCurrentBlockPosition && _currentBlock.GetComponent<Stone>().isMovementEnabled) {
                 _currentBlockMoving = true;
            }
        }
    }

    private void displayScore(ScoreModel score) {
        _score.redPoints += score.redPoints;
        _score.bluePoints += score.bluePoints;
        Player1Score.SetText(_score.redPoints.ToString());
        Player2Score.SetText(_score.bluePoints.ToString());
        Player1Score.ForceMeshUpdate(true);
        Player2Score.ForceMeshUpdate(true);
    }

    private int getBlocksInGameCount() {
        return _redBlocksInGame.Count + _blueBlocksInGame.Count;
    }

    private void switchTeam() {
        _currentTeam = _currentTeam == Team.RED ? Team.BLUE : Team.RED;
    }

    public enum Team {
        RED, BLUE
    }
}
