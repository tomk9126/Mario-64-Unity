 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 public class FreezeRot : MonoBehaviour {
 
     Transform t;
     public float fixedRotation = 0;
 
      void Start () {
         t = transform;
     }
     
      void Update () {
         t.eulerAngles = new Vector3 (fixedRotation, t.eulerAngles.y, t.eulerAngles.z);
     }
 }