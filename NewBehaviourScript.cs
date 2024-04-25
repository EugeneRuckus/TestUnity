
using System;
using System.Collections;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
private float speed = 2.0f;
Vector3 moveDirection;

private float jumpForce = 750f;
private double currSpeed;

private float mouseX = Vector3.forward.x;
private float mouseY = Vector3.forward.y;
private bool isThirdPerson;

Vector3 currDirection;
[SerializeField]private Camera mainCam;
[SerializeField]private Transform playerTrans;
private bool isOnGround;

private float maxSpeed = 200f;

    void Start(){ // This runs before first frame
        GetComponent<Rigidbody>().freezeRotation = true;
        isThirdPerson = false;

        //Physics.gravity = new Vector3(0, -15, 0); // Object gravity

        StartCoroutine(calculateSpeed());
        StartCoroutine(calculateDirection());
        StartCoroutine(jump());

        // Setup cursor
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        isOnGround = false;

    }

    void Update() // Updates once per framce
    {
        // Add mouse capability
         mouseX += Input.GetAxis("Mouse X") * Time.deltaTime * 2;
         mouseY += Input.GetAxis("Mouse Y") * Time.deltaTime * 2;
        // Make character move around
        UnityEngine.Quaternion aQuat = GetComponent<Rigidbody>().rotation;
        GetComponent<Rigidbody>().rotation = UnityEngine.Quaternion.Euler(aQuat.x + (mouseY * -10), aQuat.y + (mouseX * 10), aQuat.z);

        sprintFunc();
    }
    void LateUpdate(){
         cameraFunc();
    }
    void FixedUpdate(){
        wasdFunc();
        groundCheck();
    }

    private void sprintFunc(){
        
        if(Input.GetKeyDown(KeyCode.LeftShift)){ // Sprinting on shift
            speed = 6.0f;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)){ // Sprinting on shift
            speed = 2.0f;

        }
    }

    private void cameraFunc(){
        if(Input.GetKeyDown(KeyCode.C) && isThirdPerson == false){ // Go to third person on keypress
            Destroy(GetComponent<Camera>());
            
            isThirdPerson = true;
        }else if(Input.GetKeyDown(KeyCode.C) && isThirdPerson == true){ // Go to first person on keypress
            GetComponent<Rigidbody>().AddComponent<Camera>();
            GetComponent<Camera>().enabled = true;
            isThirdPerson = false;
        }
        if(isThirdPerson == true){ // Logic for third person camera
                GetComponent<Rigidbody>().transform.rotation = mainCam.transform.rotation; // Set the camera and player to look in same direction
                Vector3 currPlayerVec = GetComponent<Rigidbody>().position;
                // Make the camera always behind the player
                mainCam.transform.position = new Vector3(currPlayerVec.x + GetComponent<Rigidbody>().transform.right.z * 2f
                    ,currPlayerVec.y + GetComponent<Rigidbody>().transform.up.y * 1.5f, 
                    currPlayerVec.z + GetComponent<Rigidbody>().transform.right.x * -2f);
                Quaternion aQuat = GetComponent<Rigidbody>().rotation;
                // Add mouse functionality to the camera
                mainCam.transform.rotation = UnityEngine.Quaternion.Euler(aQuat.x + (mouseY * -10), aQuat.y + (mouseX * 10), aQuat.z);
        }
    }
    private void wasdFunc(){
        
        if(Input.GetKey(KeyCode.W)){ // Move forward on keypress
            moveDirection = new Vector3(playerTrans.forward.x, 0, playerTrans.right.x);
            GetComponent<Rigidbody>().AddForce(moveDirection.normalized * speed * 10, ForceMode.Force);
        
        }
        if(Input.GetKey(KeyCode.A)){ // Move left on keypress
            moveDirection = new Vector3(playerTrans.right.x * -1, 0, playerTrans.right.z * -1);
            GetComponent<Rigidbody>().AddForce(moveDirection.normalized * speed * 10, ForceMode.Force);
        
        }
        if(Input.GetKey(KeyCode.S)){ // Move backwards on keypress
            moveDirection = new Vector3(playerTrans.forward.x * -1, 0, playerTrans.right.x * -1);
            GetComponent<Rigidbody>().AddForce(moveDirection.normalized * speed * 10, ForceMode.Force);
        
        }
        if(Input.GetKey(KeyCode.D)){ //Move right on keypress
            moveDirection = new Vector3(playerTrans.right.x, 0, playerTrans.right.z);
            GetComponent<Rigidbody>().AddForce(moveDirection.normalized * speed * 10, ForceMode.Force);
        
        }
        
        if(currSpeed > maxSpeed){ // Keep the player from going too fast
            GetComponent<Rigidbody>().AddForce(currDirection.normalized * speed * -10, ForceMode.Force);
        }

    }

    private void groundCheck(){ // Check if the player is close enough to the ground
        if(UnityEngine.Physics.Raycast(GetComponent<Rigidbody>().position, transform.up * -1, 1.1f, Physics.AllLayers)){
            isOnGround = true;
        }else{
            isOnGround = false;
        }
    }
    
    IEnumerator calculateSpeed(){ // Function for calculating speed
        while(true){
            Vector3 lastVec3 = transform.position;

            yield return new WaitForSeconds(0.1f);

            currSpeed = Vector3.Distance(lastVec3, transform.position) / Time.deltaTime;
        }
    }
    IEnumerator calculateDirection(){ // Function for calculating direction of player
        while(true){
            Vector3 lastVec3 = transform.position;

            yield return new WaitForSeconds(0.1f);

            Vector3 currVec3 = transform.position;
            currDirection = new Vector3(currVec3.x - lastVec3.x, currVec3.y - lastVec3.y, currVec3.z - lastVec3.z);
        }
    }
    IEnumerator jump(){ // Function for allowing the player to hold jump
        while(true){
            if(Input.GetKey(KeyCode.Space) && (isOnGround  || wallCheck())){
                GetComponent<Rigidbody>().AddForce(0, jumpForce, 0);
                
            }
            yield return new WaitForSeconds(0.1f);
        }
        
    }
    public bool wallCheck(){
        if(UnityEngine.Physics.Raycast(GetComponent<Rigidbody>().position, transform.forward, 1.1f, Physics.AllLayers)){
            return true;
        }else if(UnityEngine.Physics.Raycast(GetComponent<Rigidbody>().position, transform.forward * -1, 1.1f, Physics.AllLayers)){
            return true;
        }else if(UnityEngine.Physics.Raycast(GetComponent<Rigidbody>().position, transform.right, 1.1f, Physics.AllLayers)){
            return true;
        }else if(UnityEngine.Physics.Raycast(GetComponent<Rigidbody>().position, transform.right * -1, 1.1f, Physics.AllLayers)){
            return true;
        }else{
            return false;
        }
    }
}
