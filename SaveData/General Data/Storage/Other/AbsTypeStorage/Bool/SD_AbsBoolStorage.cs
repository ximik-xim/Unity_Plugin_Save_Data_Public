using UnityEngine;

/// <summary>
/// Эта промежуточная абстракция нужна для возможсти использования хранилищ одного типа, но разных подходов хранения и обработки данных
/// </summary>
public abstract class SD_AbsBoolStorage : SD_AbsStorage<SD_KeyStorageBoolVariable, bool>
{
   
}
