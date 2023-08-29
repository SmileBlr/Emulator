using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingToTarget : MonoBehaviour
{
    private new Transform transform;
    private bool isNeedLooking;
    private bool isReload;

    [SerializeField] private float ReloadTime;
    [SerializeField] private float RotatingSpeed;
    [SerializeField] private AudioSource FireAudio;
    [SerializeField] private ParticleSystem FireParticle;
    [SerializeField] private GameObject AmmoPrefab;

    private void Awake()
    {
        this.transform = gameObject.transform;
    }

    #region Rotating
    public void StartLooking(Transform target)
    {
        isNeedLooking = true;

        var lookingToTarget = LookingRoutine(target);
        StartCoroutine(lookingToTarget);
    }

    public void StopLooking() => isNeedLooking = false;

    private IEnumerator LookingRoutine(Transform target)
    {
        while (isNeedLooking)
        {
            var angles = Vector3.Lerp(transform.rotation.eulerAngles, target.rotation.eulerAngles,
                RotatingSpeed * Time.deltaTime);
            transform.rotation.eulerAngles.Set(angles.x, angles.y, angles.z);

            yield return null;
        }
    }
    #endregion

    #region Shooting

    public void UpdateCanShootStatus(GameObject button) => button.SetActive(!isReload);

    public void Fire(GameObject button)
    {
        var fire = FireRoutine(button);
        StartCoroutine(fire);
    }

    private IEnumerator FireRoutine(GameObject button)
    {
        FireAudio.Play();
        FireParticle.Play();

        //TODO: CannonShoot

        isReload = true;
        UpdateCanShootStatus(button);

        yield return new WaitForSeconds(ReloadTime);

        isReload = false;
        UpdateCanShootStatus(button);
    }
    #endregion
}
