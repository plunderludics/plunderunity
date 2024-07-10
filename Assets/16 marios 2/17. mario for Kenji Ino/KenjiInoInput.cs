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
                keyName = "K",
                isPressed = true
            });
            firstPerson = false;
        } else {
             AddInputEvent(new InputEvent() {
                keyName = "K",
                isPressed = false
            });
        }


            if (move.y != 0) {
                AddInputEvent(new InputEvent() {
                    keyName = move.y > 0 ? stuff.Uparrow : stuff.Downarrow,
                    isPressed = true
                });
            }

            if (jump) {
                AddInputEvent(new InputEvent() {
                    keyName = "A",
                    isPressed = true
                });
            }

            if (move.x != 0) {
                AddInputEvent(new InputEvent() {
                    keyName = move.x < 0 ? stuff.Leftarrow : stuff.Rightarrow,
                    isPressed = true
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
                keyName = "I",
                isPressed = true
            });
            firstPerson = true;
        } else {
             AddInputEvent(new InputEvent() {
                keyName = "I",
                isPressed = false
            });
        }

        AddInputEvent(new InputEvent() {
            keyName = "A",
            isPressed = false
        });

        AddInputEvent(new InputEvent() {
            keyName = stuff.Downarrow,
            isPressed = false
        });

        AddInputEvent(new InputEvent() {
            keyName = stuff.Uparrow,
            isPressed = false
        });

        AddInputEvent(new InputEvent() {
            keyName = stuff.Leftarrow,
            isPressed = false
        });

        AddInputEvent(new InputEvent() {
            keyName = stuff.Rightarrow,
            isPressed = false
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