using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class GunBehavior : MonoBehaviour
{

    [SerializeField] private Transform m_hitPrefab;

    [SerializeField] private int m_roundsPerMinute = 600;

    // Time to reset (decay) the recoil
    [SerializeField] private float m_decayTime = 0.5f;

    // Factor time to return original position when new shot comes out
    [SerializeField] private float m_decayFactorTime = 2f;

    [SerializeField] private float m_reloadTime = 1f;

    [SerializeField] private float m_recoilAnimationTime = 0.5f;

    [SerializeField] private List<Vector3> m_recoilEulers = new List<Vector3>();

    [SerializeField] private FirstPersonController m_FPSController;

    [SerializeField] private AudioClip m_shootAudio;

    [SerializeField] private AudioClip m_reloadAudio;

    [SerializeField] private Vector3 m_recoiledGunPosition = new Vector3(0.291f, 0.373f, 0.558f);

    [SerializeField] private Text m_ammoText;

    [SerializeField] private MuzzleFlashBehavior m_muzzleFlash;

    [SerializeField] private float m_xSensitivity = 2f;

    [SerializeField] private float m_ySensitivity = 2f;

    private Vector3 rayDirection;

    private int RPIndex = 0;

    private GameObject cam;

    private float m_shotDelay = 0f;

    private float m_shotAccumulatedTime = 0f;

    private bool m_onDecay = false;

    private Vector3 m_originPoint;

    private Vector3 m_originVector;

    private Vector3 m_decayPoint;

    private float m_decayAccumulatedTime = 0f;

    private AudioSource m_gunAudioSource;

    private bool m_onReload = false;

    private float m_reloadAccumulatedTime = 0f;

    private Vector3 m_restGunLocalPosition;

    private bool m_recoilAnimation = false;

    private float m_recoilAnimationAccTime = 0f;


    // Start is called before the first frame update
    void Start()
    {

        m_gunAudioSource = GetComponent<AudioSource>();

        m_restGunLocalPosition = transform.localPosition;

        m_ammoText.text = "30 / 30";

        m_shotDelay = 1 / ((float) m_roundsPerMinute / 60);

        cam = GameObject.FindWithTag("MainCamera");

        // Adding trivial euler angles to tilt camera when shooting to predictive positions
        // Comment this lines if want to make custom tilt angles (modifiable in the editor)
        m_recoilEulers.Add(new Vector3(359.7f, 0.1f, 0f));
        m_recoilEulers.Add(new Vector3(358.9f, 0f, 0f));
        m_recoilEulers.Add(new Vector3(357.7f, 0.1f, 0f));
        m_recoilEulers.Add(new Vector3(356.5f, 0.1f, 0f));
        m_recoilEulers.Add(new Vector3(355.1f, 359.6f, 0f));
        m_recoilEulers.Add(new Vector3(353.9f, 359.3f, 0f));
        m_recoilEulers.Add(new Vector3(353.1f, 358.7f, 0f));
        m_recoilEulers.Add(new Vector3(352.5f, 359.4f, 0f));
        m_recoilEulers.Add(new Vector3(352.7f, 1.2f, 0f));
        m_recoilEulers.Add(new Vector3(352.6f, 2f, 0f));
        m_recoilEulers.Add(new Vector3(352.2f, 1.6f, 0f));
        m_recoilEulers.Add(new Vector3(351.9f, 2.2f, 0f));
        m_recoilEulers.Add(new Vector3(352.3f, 3.2f, 0f));
        m_recoilEulers.Add(new Vector3(352.1f, 3.4f, 0f));
        m_recoilEulers.Add(new Vector3(352f, 1.7f, 0f));
        m_recoilEulers.Add(new Vector3(351.7f, 0.9f, 0f));
        m_recoilEulers.Add(new Vector3(351.3f, 0.3f, 0f));
        m_recoilEulers.Add(new Vector3(351.4f, 359.2f, 0f));
        m_recoilEulers.Add(new Vector3(351.7f, 357.8f, 0f));
        m_recoilEulers.Add(new Vector3(351.8f, 358.7f, 0f));
        m_recoilEulers.Add(new Vector3(351.7f, 358.5f, 0f));
        m_recoilEulers.Add(new Vector3(351.3f, 358.7f, 0f));
        m_recoilEulers.Add(new Vector3(351.2f, 359.1f, 0f));
        m_recoilEulers.Add(new Vector3(351.3f, 358.3f, 0f));
        m_recoilEulers.Add(new Vector3(351.1f, 358f, 0f));
        m_recoilEulers.Add(new Vector3(351.1f, 358.9f, 0f));
        m_recoilEulers.Add(new Vector3(351.2f, 0.2f, 0f));
        m_recoilEulers.Add(new Vector3(352.1f, 2.1f, 0f));
        m_recoilEulers.Add(new Vector3(352.1f, 2.8f, 0f));

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire1") && !m_onReload && m_shotAccumulatedTime <= 0f)
        {

            if (RPIndex <= m_recoilEulers.Count)
            {

                m_gunAudioSource.Stop();
                m_gunAudioSource.clip = m_shootAudio;
                m_gunAudioSource.Play();
                Shoot();
                SetAmmoText((m_recoilEulers.Count + 1) - (RPIndex + 1));
                m_muzzleFlash.Flash();
                m_shotAccumulatedTime = m_shotDelay;
                RPIndex++;
                m_recoilAnimationAccTime = 0f;
                m_recoilAnimation = true;

            }
            else
            {
                m_gunAudioSource.Stop();
                m_gunAudioSource.clip = m_reloadAudio;
                m_gunAudioSource.Play();
                m_onReload = true;

            }

        }
        else
        {

            m_shotAccumulatedTime -= Time.deltaTime;

        }

        if (Input.GetButtonUp("Fire1"))
        {

            

        }
        
        if (m_onDecay)
        {

            if (m_decayAccumulatedTime < m_decayTime)
            {

                UpdateOrigin();
                DecayRecoil( (m_decayAccumulatedTime / m_decayFactorTime) );
                m_decayAccumulatedTime += Time.deltaTime;

            }
            else
            {
                RPIndex = 0;
                m_onDecay = false;

            }

        }

        if (m_recoilAnimation)
        {

            if (m_recoilAnimationAccTime < m_recoilAnimationTime)
            {

                if (m_recoilAnimationAccTime < (m_recoilAnimationTime / 2))
                {

                    AnimateGunRecoil((m_recoilAnimationAccTime / (m_recoilAnimationTime / 2)), true);

                }
                else
                {
                    float animFactor = m_recoilAnimationAccTime / (m_recoilAnimationTime / 2) - m_recoilAnimationTime;
                    AnimateGunRecoil(animFactor, false);

                }

                m_recoilAnimationAccTime += Time.deltaTime;

            }
            else
            {
                transform.localPosition = m_restGunLocalPosition;
                m_recoilAnimation = false;
                m_recoilAnimationAccTime = 0f;

            }

        }

        if (m_onReload)
        {

            if (m_reloadAccumulatedTime < m_reloadTime)
            {

                m_reloadAccumulatedTime += Time.deltaTime;

            }
            else
            {

                RPIndex = 0;
                m_onReload = false;
                m_reloadAccumulatedTime = 0f;
                SetAmmoText(30);

            }

        }


    }

    private void Shoot()
    {

        if (m_onDecay)
        {

            Vector3 crosshairPoint = cam.transform.position + (cam.transform.TransformDirection(Vector3.forward) * 10);
            Vector3 wallVector = (crosshairPoint - m_originPoint) * 2f;
            Vector3 rayPoint = m_originPoint + wallVector;
            rayDirection = rayPoint - cam.transform.position;

        }
        else
        {

            m_originVector = cam.transform.TransformDirection(Vector3.forward);
            m_originPoint = cam.transform.position + (m_originVector * 10);
            rayDirection = m_originVector;

        }

        RaycastHit bulletHit;
        if (Physics.Raycast(cam.transform.position, rayDirection, out bulletHit, Mathf.Infinity))
        {

            Vector3 realHit = bulletHit.point;
            realHit.z -= 0.01f;
            Instantiate(m_hitPrefab, realHit, m_hitPrefab.rotation);

        }
        else
        {
            Debug.DrawRay(cam.transform.position, rayDirection * 1000, Color.white);
            Debug.Log("BulletHit Did not Hit");
        }

        if (RPIndex <= (m_recoilEulers.Count - 1))
        {

            Vector3 cameraVector = Quaternion.Euler(m_recoilEulers[RPIndex]) * m_originVector;
            cam.transform.LookAt(cam.transform.position + (cameraVector * 10));
            m_FPSController.m_MouseLook.Init(m_FPSController.transform, cam.transform);
            m_decayPoint = cam.transform.position + (cameraVector * 10);
            m_onDecay = true;
            m_decayAccumulatedTime = 0;

        }

    }

    private void DecayRecoil(float decayFactor)
    {

        m_decayPoint = Vector3.Lerp(m_decayPoint, m_originPoint, decayFactor);
        cam.transform.LookAt(m_decayPoint);
        m_FPSController.m_MouseLook.Init(m_FPSController.transform, cam.transform);

    }

    private void UpdateOrigin()
    {

        float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * m_xSensitivity;
        float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * m_ySensitivity;

        m_originVector = Quaternion.Euler(-xRot, yRot, 0f) * m_originVector;
        m_originPoint = cam.transform.position + (m_originVector * 10);

    }

    private void AnimateGunRecoil(float animFactor, bool animState)
    {

        if (animState)
        {
            transform.localPosition = Vector3.Lerp(m_restGunLocalPosition, m_recoiledGunPosition, animFactor);

        }
        else
        {

            transform.localPosition = Vector3.Lerp(m_recoiledGunPosition, m_restGunLocalPosition, animFactor);

        }

    }

    private void SetAmmoText(int ammo)
    {

        m_ammoText.text = ammo.ToString() + " / 30";

    }

}
