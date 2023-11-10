
public class AttackCommand : ICommand
{
    private BattleCharacter _attacker;
    private BattleCharacter _target;

    public AttackCommand(BattleCharacter attacker, BattleCharacter target)
    {
        _attacker = attacker;
        _target = target;
    }

    public void Execute()
    {
        _attacker.MoveToAttack(_target.GetAttackPointPosition());
        _attacker.PlayAttackAnimation();
        if (!CalculateIfHit(_attacker.AttackHitPercentage, _target.AttackAvoidPercentage))
        {
            //handle miss 
            return;
        }

        //Addition logic should be handled here somehow
        CalculateAttackDamage(_attacker.Attack, _target.Defense);
    }

    private bool CalculateIfHit(int attackerHitChance, int targetEvadeChance)
    {
        //just always hit for now
        return true;
    }
    private int CalculateAttackDamage(int attackerAttack, int targetDefense)
    {
        //very simple for right now just to get things working
        var damageTaken = targetDefense - attackerAttack;
        return damageTaken <= 0 ? 0 : damageTaken;
    }
}