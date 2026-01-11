

using NUnit.Framework;

using UnityEngine;

public class AttackResolverTests { 

    private DetailedWeapon firearm;
    private FirearmAttack attack; 

    [SetUp]
    public void SetUp() {
        firearm = GameObject.Instantiate(Resources.Load<DetailedWeapon>("gun_california"));
        firearm.bullet_effect = GameObject.Instantiate(firearm.bullet_effect);
        attack = new FirearmAttack();
        attack.firearm = firearm;
    }
    
    // [Test]
    // public void TestSetup() {
        
    // }
    [Test]
    public void TestNoneDamageFalloff() {
        firearm.bullet_effect.damage_falloff_function = DecayFunction.none;
        firearm.bullet_effect.damage_falloff_rate = 1;
        Assert.AreEqual(0, AttackResolver.GetDamageFalloff(attack, 1));
        Assert.AreEqual(0, AttackResolver.GetDamageFalloff(attack, 2));
        
        firearm.bullet_effect.damage_falloff_rate = 2;
        Assert.AreEqual(0, AttackResolver.GetDamageFalloff(attack, 3));
        Assert.AreEqual(0, AttackResolver.GetDamageFalloff(attack, 4));
    }

    [Test]
    public void TestConstantDamageFalloff()
    {
        firearm.bullet_effect.damage_falloff_function = DecayFunction.constant;
        firearm.bullet_effect.damage_falloff_rate = 1;
        Assert.AreEqual(1, AttackResolver.GetDamageFalloff(attack, 1));
        Assert.AreEqual(1, AttackResolver.GetDamageFalloff(attack, 2));
        
        firearm.bullet_effect.damage_falloff_rate = 2;
        Assert.AreEqual(2, AttackResolver.GetDamageFalloff(attack, 3));
        Assert.AreEqual(2, AttackResolver.GetDamageFalloff(attack, 4));
    }
    
    [Test]
    public void TestLogrithmicDamageFalloff()
    {
        firearm.bullet_effect.damage_falloff_function = DecayFunction.logrithmic;
        firearm.bullet_effect.damage_falloff_rate = 2;

        Assert.AreEqual(1.3862944f, AttackResolver.GetDamageFalloff(attack, 2), delta:0.01f);
        Assert.AreEqual(2.7725887f, AttackResolver.GetDamageFalloff(attack, 4), delta:0.01f);
        firearm.bullet_effect.damage_falloff_rate = 5;
        Assert.AreEqual(6.9314718f, AttackResolver.GetDamageFalloff(attack, 4), delta:0.01f);
        Assert.AreEqual(10.3972077f, AttackResolver.GetDamageFalloff(attack, 8), delta:0.01f);
    }
    
    [Test]
    public void TestLinearDamageFalloff()
    {
        firearm.bullet_effect.damage_falloff_function = DecayFunction.linear;
        firearm.bullet_effect.damage_falloff_rate = 2;
        Assert.AreEqual(2, AttackResolver.GetDamageFalloff(attack, 1));
        Assert.AreEqual(4, AttackResolver.GetDamageFalloff(attack, 2));
        
        firearm.bullet_effect.damage_falloff_rate = 3;
        Assert.AreEqual(9, AttackResolver.GetDamageFalloff(attack, 3));
        Assert.AreEqual(12, AttackResolver.GetDamageFalloff(attack, 4));
    }
    
    [Test]
    public void TestExponentialDamageFalloff()
    {
        firearm.bullet_effect.damage_falloff_function = DecayFunction.exponential;
        firearm.bullet_effect.damage_falloff_rate = 2;
        Assert.AreEqual(2 * Mathf.Exp(1), AttackResolver.GetDamageFalloff(attack, 1), delta:0.01f);
        Assert.AreEqual(2 * Mathf.Exp(2), AttackResolver.GetDamageFalloff(attack, 2), delta:0.01f);
        
        firearm.bullet_effect.damage_falloff_rate = 3;
        Assert.AreEqual(3 * Mathf.Exp(2), AttackResolver.GetDamageFalloff(attack, 2), delta:0.01f);
        Assert.AreEqual(3 * Mathf.Exp(3), AttackResolver.GetDamageFalloff(attack, 3), delta:0.01f);
    }

    [Test]
    public void TestLogLessThanOne() {
        firearm.bullet_effect.damage_falloff_function = DecayFunction.logrithmic;
        firearm.bullet_effect.damage_falloff_rate = 1;

        Assert.AreEqual(0f, AttackResolver.GetDamageFalloff(attack, 0), delta:0.01f);
        Assert.AreEqual(0f, AttackResolver.GetDamageFalloff(attack, 0.99f), delta:0.01f);
        Assert.AreEqual(0f, AttackResolver.GetDamageFalloff(attack, -1f), delta:0.01f);
        Assert.AreEqual(0f, AttackResolver.GetDamageFalloff(attack, 1f), delta:0.01f);
    }

    [Test]
    public void LogAndExponentInverse() {
        firearm.bullet_effect.damage_falloff_function = DecayFunction.exponential;
        firearm.bullet_effect.damage_falloff_rate = 1;

        float input_val = 10f;
        float result = AttackResolver.GetDamageFalloff(attack, input_val);

        firearm.bullet_effect.damage_falloff_function = DecayFunction.logrithmic;
        result = AttackResolver.GetDamageFalloff(attack, result);
        Assert.AreEqual(input_val, result);
    }
}