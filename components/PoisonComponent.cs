using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Objects.Characters;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters.components;

public class PoisonComponent : MonoBehaviour
{
    private float _lastFired = 0f;
    private float _buff;
    private EnemyController _enemy;

    public PoisonComponent(IntPtr ptr) : base(ptr) {}
    
    public void Initialize(EnemyController enemy, float buff)
    {
        _enemy = enemy;
        _buff = buff;
    }

    public void Update()
    {
        _enemy._EnemyRenderer.SetTint(new Il2CppSystem.Nullable<Color>(new Color(0.4f, 0.7f, 0.0f)));
        if (!_enemy || _enemy.IsDead)
        {
            Remove();
            return;
        }

        if (Time.time - _lastFired >= 0.5f)
        {
            _lastFired = Time.time;
            _enemy.GetDamaged(5f * _buff);
        }
    }

    public void Remove()
    {
        Destroy(this);
    }
}