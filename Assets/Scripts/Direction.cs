using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour {
    public enum Horizontal { Right, Left }

    [SerializeField] Horizontal initialDirection = Horizontal.Right;

    Horizontal facing;

    private void Awake() {
        Init();
    }

    public void Init() {
        SetFacing(initialDirection);
    }
    public bool FacingRight() {
        return (facing == Horizontal.Right);
    }

    public bool FacingLeft() {
        return (facing == Horizontal.Left);
    }

    public void SetFacing(Horizontal direction) {
        if (facing == direction) return;
        transform.Rotate(0, 180, 0);

        facing = direction;
    }

    public void Reverse() {
        SetFacing(Opposite(facing));
    }

    public static Horizontal Opposite(Horizontal direction) {
        return (direction == Horizontal.Left) ? Horizontal.Right : Horizontal.Left;
    }

    public Direction.Horizontal GetFacing() {
        return facing;
    }

    public Vector2 FowardVector() {
        return FacingRight() ? Vector2.right : Vector2.left;
    }

    public bool FacingToward(GameObject other) {
        if (!other) return false;
        return facing == DirectionToward(other.transform.position);
    }

    public void FaceToward(Vector2 position) {
        SetFacing(DirectionToward(position));
    }

    public void FaceToward(GameObject other) {
        if (!other) return;
        SetFacing(DirectionToward(other.transform.position));
    }

    public Direction.Horizontal DirectionToward(Vector2 position) {
        if (position.x >= transform.position.x) return Direction.Horizontal.Right;
        return Direction.Horizontal.Left;
    }
}
