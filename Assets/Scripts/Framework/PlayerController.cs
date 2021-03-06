﻿using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    public GameObject PSpawn;
    public bool usingGamePad = false;

    //Controller Inputs//
    Vector2 leftStick = Vector2.zero;
    Vector2 rightStick = Vector2.zero;
    float leftTrigger = 0.0f;
    float rightTrigger = 0.0f;

    bool buttonSouth = false;
    bool buttonWest = false;
    bool buttonEast = false;
    bool buttonNorth = false;

    bool dpad_up = false;
    bool dpad_right = false;
    bool dpad_left = false;
    bool dpad_down = false;

    bool leftShoulder = false;
    bool rightShoulder = false;

    bool leftStickButton = false;
    bool rightStickButton = false;
    //Controller Inputs//

    private void Start()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        if (!IsLocalPlayer){
            return;
        }
        if (!myPawn){
            return;
        }

        GetInput();

        ((PlayerPawn)myPawn).SetCamPitch(rightStick.y);
        myPawn.RotatePlayer(rightStick.x);
        myPawn.Move(leftStick.x, leftStick.y);

    }

    

    private void GetInput()
    {
        leftStick = Vector2.zero;
        rightStick = Vector2.zero;

        if(usingGamePad)
        {
            bool gamePadConnected = GetInputGamePad();
            if (gamePadConnected)
            {
                return;
            }
        }

        GetInputKeyboardMouse();


    }

    private bool GetInputGamePad()
    {
        Gamepad gPad = Gamepad.current;

        if(gPad == null)
        {
            return false;
        }

        //TO-DO adding gamepad input here
        //
        //
        //
        //

        return true;
    }

    private void GetInputKeyboardMouse()
    {
        Keyboard KB = Keyboard.current;
        Mouse mouse = Mouse.current;


        rightStick.x = Input.GetAxis("Mouse X");
        rightStick.y = Input.GetAxis("Mouse Y");
        dpad_up = Input.GetKey(KeyCode.W);
        dpad_down = Input.GetKey(KeyCode.S);
        dpad_left = Input.GetKey(KeyCode.A);
        dpad_right = Input.GetKey(KeyCode.D);
        buttonSouth = Input.GetKeyDown(KeyCode.E);

        KeyToAxis();
    }

    private void KeyToAxis()
    {
        if (dpad_up)
        {
            leftStick.y = 1;
        }
        if (dpad_down)
        {
            leftStick.y = -1;
        }
        if (dpad_right)
        {
            leftStick.x = 1;
        }
        if (dpad_left)
        {
            leftStick.x = -1;
        }
    }

    public void SpawnPlayer()
    {
        if (IsOwner)
        {
            InvokeServerRpc(Server_SpawnPlayer, OwnerClientId);
        }
    }

    [ServerRPC(RequireOwnership = false)]
    public void Server_SpawnPlayer(ulong whclient)
    {
        Debug.Log("Server_SpawnPlayer called");
        Vector3 position = new Vector3(0, 0, 0);
        GameObject Gobj = Instantiate(PSpawn, position, Quaternion.identity);
        Gobj.GetComponent<NetworkedObject>().SpawnWithOwnership(OwnerClientId);

        InvokeClientRpcOnClient(client_set, whclient, Gobj.GetComponent<NetworkedObject>().NetworkId);
    }

    [ClientRPC]
    public void client_set(ulong id)
    {
        Debug.Log("client_set");
        myPawn = GetNetworkedObject(id).GetComponent<PlayerPawn>();
        myPawn.Possessed(this);
        myPawn.CameraControl.SetActive(true);
    }

        
    
        
}
