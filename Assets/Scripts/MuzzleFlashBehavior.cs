using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashBehavior : MonoBehaviour
{

    [SerializeField] private float m_flashTime = 0.1f;

    private float m_flashAccumulatedTime = 0f;

    private bool m_onFlash = false;

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Renderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

        if (m_onFlash)
        {

            if (m_flashAccumulatedTime > m_flashTime)
            {

                GetComponent<Renderer>().enabled = false;
                m_onFlash = false;

            }
            else
            {

                m_flashAccumulatedTime += Time.deltaTime;

            }

        }

    }

    public void Flash()
    {

        GetComponent<Renderer>().enabled = true;
        m_flashAccumulatedTime = 0f;
        m_onFlash = true;

    }

}
