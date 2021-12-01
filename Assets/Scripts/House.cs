using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    private Collider _houseCollider;
    void Start() {
        _houseCollider = gameObject.GetComponent<CapsuleCollider>();
    }
    void Update() {}

    public bool isAnyBlockInHouse(IList<GameObject> redblocks, IList<GameObject> blueblocks) {
        foreach (var item in redblocks)
        {
            var itemCollider = item.GetComponent<CapsuleCollider>();
            if(_houseCollider.bounds.Intersects(itemCollider.bounds)) {
                return true;
            }
        }
        foreach (var item in blueblocks)
        {
            var itemCollider = item.GetComponent<CapsuleCollider>();
            if(_houseCollider.bounds.Intersects(itemCollider.bounds)) {
                return true;
            }
        }
        return false;
    }

    public ScoreModel calculateEndsScore(IList<GameObject> redblocks, IList<GameObject> blueblocks) {

        var closestTeam = GameBoard.Team.RED;
        var closestDistance = GetDistanceTo(redblocks[0]);
        
        foreach(var item in redblocks) {
            var distance = GetDistanceTo(item);
            if(distance < closestDistance) {
                closestDistance = distance;
                closestTeam = GameBoard.Team.RED;
            }
        }
        foreach(var item in blueblocks) {
            var distance = GetDistanceTo(item);
            if(distance < closestDistance) {
                closestDistance = distance;
                closestTeam = GameBoard.Team.BLUE;
            }
        }
        
        var loosersList = closestTeam == GameBoard.Team.RED ? blueblocks : redblocks;
        var winnersList = closestTeam == GameBoard.Team.BLUE ? blueblocks : redblocks;
        var closestLooserDistance = GetDistanceTo(loosersList[0]);

        foreach(var item in loosersList) {
            var distance = GetDistanceTo(item);
            if(distance < closestLooserDistance) {
                closestLooserDistance = distance;
            }
        }

        var pointsAccumulated = 0;

        foreach(var item in winnersList) {
            var distance = GetDistanceTo(item);
            if(distance < closestLooserDistance) {
                pointsAccumulated++;
            }
        }

        if(closestTeam == GameBoard.Team.RED) {
            return new ScoreModel(pointsAccumulated, 0);
        }
        return new ScoreModel(0, pointsAccumulated);
    }

    private float GetDistanceTo(GameObject gameObject) {
        var housePosition = new Vector2(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.z);
        var objectPosition = new Vector2(gameObject.transform.localPosition.x, gameObject.transform.localPosition.z);
        return Vector2.Distance(housePosition, objectPosition);
    }
}
