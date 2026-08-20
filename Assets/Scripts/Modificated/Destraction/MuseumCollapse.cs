using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class MuseumCollapse : MonoBehaviour
{
    [SerializeField] Rigidbody _mesh;
    [SerializeField] float _delay = 1.0f;
    [SerializeField] Rigidbody[] _rbs;
    [SerializeField] Lamp[] _lamps;
    [SerializeField] VisualEffect[] _vfxs;
    [SerializeField] AudioSource _as;
    [SerializeField] float _impusleForce = 0.01f;
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
            lamps.TakeImpulse(_impusleForce);
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