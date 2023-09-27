//consider scrapping
public interface ICharacterBattleActions
{ 
    public void Defend();
    public void TakeDamage(int damage);
}

public interface IAffectedByAilments
{
    public void ApplyPreTurnStatusEffects();
    public void ApplyPostTurnStatusEffects();
}