using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{

    public GameObject firePosition;

    public GameObject bombFactory;

    public float throwPower = 15f;

    public GameObject bulletEffect;

    public int weaponPower = 5;

    public GameObject[] eff_Flash;

    public GameObject weapon01;
    public GameObject weapon02;

    public GameObject crosshair01;
    public GameObject crosshair02;

    public GameObject weapon01_R;
    public GameObject weapon02_R;

    public GameObject crosshair02_zoom;

    ParticleSystem ps;

    Animator anim;

    enum WeaponMode
    {
        Normal,
        Sniper
    }

    WeaponMode wMode;

    bool ZoomMode = false;

    public Text wModeText;

    // Start is called before the first frame update
    void Start()
    {
        ps = bulletEffect.GetComponent<ParticleSystem>();

        anim = GetComponentInChildren<Animator>();

        wMode = WeaponMode.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon01.SetActive(true);
            weapon02.SetActive(false);
            crosshair01.SetActive(true);
            crosshair02.SetActive(false);
            weapon01_R.SetActive(true);
            weapon02_R.SetActive(false);

            crosshair02_zoom.SetActive(false);
            ZoomMode = false;

            wMode = WeaponMode.Normal;

            Camera.main.fieldOfView = 60f;

            wModeText.text = "Normal Mode";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon01.SetActive(false);
            weapon02.SetActive(true);
            crosshair01.SetActive(false);
            crosshair02.SetActive(true);
            weapon01_R.SetActive(false);
            weapon02_R.SetActive(true);

            wMode = WeaponMode.Normal;
            wMode = WeaponMode.Sniper;

            wModeText.text = "Sniper Mode";
        }

        if (Input.GetMouseButtonDown(1))
        {
            switch (wMode)
            {
                case WeaponMode.Normal:
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.transform.position;

                    Rigidbody rb = bomb.GetComponent<Rigidbody>();

                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
                    break;
                case WeaponMode.Sniper:
                    if (!ZoomMode)
                    {
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = true;
                        crosshair02_zoom.SetActive(true);
                        crosshair02.SetActive(false);
                    }
                    else
                    {
                        Camera.main.fieldOfView = 60f;
                        ZoomMode = false;
                        crosshair02_zoom.SetActive(false);
                        crosshair02.SetActive(true);
                    }
                    break;
            }

        }

        if (Input.GetMouseButtonDown(0))
        {
            if (anim.GetFloat("MoveMotion") == 0)
            {
                anim.SetTrigger("Attack");
            }

            StartCoroutine(ShootEffectOn(0.05f));

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Raycast(ray, out hitInfo))
            {
                if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(weaponPower);
                }
                else
                {
                    bulletEffect.transform.position = hitInfo.point;
                    bulletEffect.transform.forward = hitInfo.normal;
                    ps.Play();
                }
            }
        }
    }

    IEnumerator ShootEffectOn(float duration)
    {
        int num = Random.Range(0, eff_Flash.Length - 1);

        eff_Flash[num].SetActive(true);

        yield return new WaitForSeconds(duration);

        eff_Flash[num].SetActive(false);
    }
}
