using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteBlend : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private string blendProperty = "_Blend";
    [SerializeField] private float changeThreshold = 0.0005f;

    private SpriteRenderer _sr;
    private MaterialPropertyBlock _mpb;
    private float _lastBlend = -1f;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _mpb = new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        float blend = player.MeterNormalized;

        if (Mathf.Abs(blend - _lastBlend) < changeThreshold)
            return;

        _lastBlend = blend;

        _sr.GetPropertyBlock(_mpb);
        _mpb.SetFloat(blendProperty, blend);
        _sr.SetPropertyBlock(_mpb);
    }
}
