using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightCamera : MonoBehaviour {

    // Character
    [SerializeField]
    private PlayerController m_Character = null;

    // Offset
    [SerializeField]
    private Vector3 m_CharacterOffset = new Vector3(0, 1, -6);

    // Lerp factor
    [SerializeField]
    private float m_LerpFactor = 6;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        Vector3 localOffset = m_Character.transform.right * m_CharacterOffset.x + m_Character.transform.up * m_CharacterOffset.y + m_Character.transform.forward * m_CharacterOffset.z;

        // Update position based on offset
        Vector3 desiredPosition = m_Character.transform.position + localOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime * m_LerpFactor);

        transform.rotation = Quaternion.Lerp(transform.rotation, m_Character.transform.rotation, Time.fixedDeltaTime * m_LerpFactor);

    }
}
