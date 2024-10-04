using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punch : MonoBehaviour
{
public Transform AttackPoint;
    public PlayerCombat Combat;
    public GameObject Punch;
    // Start is called before the first frame update
    public void TestFunction(){
        Instantiate(Punch, AttackPoint.position, AttackPoint.rotation);

    }
}
