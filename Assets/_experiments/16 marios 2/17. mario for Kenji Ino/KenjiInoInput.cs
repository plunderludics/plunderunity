using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHawk;

public class KenjiInoInput : InputProvider {
    public float _movingDelay = 2f;
    public float _pauseDelay = 0.2f;
    public int _MoveSpeed = 10;
    public float _moveTimer = 0f;
    public Emulator emu;

    public Vector2Int move;
    public bool jump;
    public bool firstPerson;
    bool _isMoving => _moveTimer > 0f;

    // Start is called before the first frame update
    void Update() {
        // if (!emu.IsRunning) return;

        var stuff = new { Uparrow = "UpArrow", Downarrow = "DownArrow", Leftarrow = "LeftArrow", Rightarrow = "RightArrow" };

        _moveTimer -= Time.deltaTime;
        if (_isMoving) {
            // emu.Unpause();
            if (firstPerson) {
                AddInputEvent(new InputEvent() {
                    name = "K",
                    value = 1,
                });
                firstPerson = false;
            } else {
                AddInputEvent(new InputEvent() {
                    name = "K",
                    value = 0,
                });
            }


            if (move.y != 0) {
                AddInputEvent(new InputEvent() {
                    name = move.y > 0 ? stuff.Uparrow : stuff.Downarrow,
                    value = 1,
                });
            }

            if (jump) {
                AddInputEvent(new InputEvent() {
                    name = "A",
                    value = 1,
                });
            }

            if (move.x != 0) {
                AddInputEvent(new InputEvent() {
                    name = move.x < 0 ? stuff.Leftarrow : stuff.Rightarrow,
                    value = 1,
                });

                // AddAxisInputEvent("X Axis", move.x * 128);
            }


            return;
        };

        move = Vector2Int.zero;
        jump = false;
        // emu.Pause();
        if (!firstPerson) {
            AddInputEvent(new InputEvent() {
                name = "I",
                value = 1,
            });

            firstPerson = true;
        } else {
            AddInputEvent(new InputEvent() {
                name = "I",
                value = 0,
            });
        }

        AddInputEvent(new InputEvent() {
            name = "A",
            value = 0,
        });

        AddInputEvent(new InputEvent() {
            name = stuff.Downarrow,
            value = 0,
        });

        AddInputEvent(new InputEvent() {
            name = stuff.Uparrow,
            value = 0,
        });

        AddInputEvent(new InputEvent() {
            name = stuff.Leftarrow,
            value = 0,
        });

        AddInputEvent(new InputEvent() {
            name = stuff.Rightarrow,
            value = 0,
        });

        if (Input.GetKeyDown(KeyCode.Space)) {
            jump = true;
            _moveTimer = _movingDelay;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            move.y = 1;
            _moveTimer = _movingDelay;
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            _moveTimer = _movingDelay;
            move.y = -1;
        }
        else if (Input.GetKeyDown(KeyCode.A)) {
            move.x = -1;
            _moveTimer = _movingDelay;
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            move.x = +1;
            _moveTimer = _movingDelay;
        }
    }
}