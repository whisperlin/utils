using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferInfor : MonoBehaviour {

    public Transform target;
    public float height = 5f;
    public float speed = 10f;

    private void Start()
    {
        

        LCharacterHitDataCmp hdc = GetComponent<LCharacterHitDataCmp>();
        if (null == hdc)
        {
            hdc = gameObject.AddComponent<LCharacterHitDataCmp>();
        }
        LCharacterColliderData cdata = hdc.data;

        LCharacterTransferData data = new LCharacterTransferData();
        cdata.type = "tra";
        data.target = target;
        data.height = height;
        data.speed = speed;
        cdata.data = data;


    }

}
