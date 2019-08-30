using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBehavior : MonoBehaviour
{

    [SerializeField] private Texture m_sparkTexture;

    [SerializeField] private Texture m_holeTexture;

    [SerializeField] private float m_textureChangeTime = 0.2f;

    private float m_textureChangeAccTime = 0f;

    private bool m_textureChanged = false;

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Renderer>().material.mainTexture = m_sparkTexture;

    }

    // Update is called once per frame
    void Update()
    {

        if (!m_textureChanged)
        {

            if (m_textureChangeAccTime > m_textureChangeTime)
            {

                GetComponent<Renderer>().material.mainTexture = m_holeTexture;
                m_textureChanged = true;

            }
            else
            {

                m_textureChangeAccTime += Time.deltaTime;

            }

        }



    }
}
