

public static class DamageResolver {

    public static void ResolveAttack(IAttack attack, 
            IAttackTarget target) {
        ICharacterStatus status = target.GetStatus();

        status.health -= attack.attack_damage;
        // TODO --- implement armor
    }
}