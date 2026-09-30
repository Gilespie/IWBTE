using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class MuseumCollapse : MonoBehaviour
{
    [SerializeField] Rigidbody _mesh;
    [SerializeField] float _delay = 1.0f;
    [SerializeField] Rigidbody[] _rbs;
    [SerializeField] Lamp[] _lamps;
    [SerializeField] Light[] _lights;
    [SerializeField] VisualEffect[] _vfxs;
    [SerializeField] AudioSource _as;
    [SerializeField] Animator _animator;

    public void ActivateCollapse()
    {
        _as.Play();

        _animator.enabled = true;

        StartCoroutine(FallRoutine());

        foreach(var vfx in _vfxs)
        {
            vfx.Play();
        }

        foreach (var rbs in _rbs)
        {
            rbs.isKinematic = false;
        }

        foreach (var lamps in _lamps)
        {
            lamps.LampEvent();
        }

        foreach(var lights in _lights)
        {
            lights.enabled = false;
        }
    }

    IEnumerator FallRoutine()
    {
        yield return new WaitForSeconds(_delay);
        _mesh.isKinematic = false;
        yield return new WaitForSeconds(_delay);
        _mesh.isKinematic = true;
        _mesh.GetComponent<MeshCollider>().isTrigger = false;

        yield return null;
    }
}