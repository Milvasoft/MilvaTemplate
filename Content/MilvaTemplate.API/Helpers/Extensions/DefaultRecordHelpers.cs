namespace MilvaTemplate.API.Helpers.Extensions;

/// <summary>
/// Helper extensions methods for Ops!yon Project.
/// </summary>
public static partial class HelperExtensions
{
    #region Default Record Check Helpers

    /// <summary>
    /// Checks <paramref name="id"/> is default record id or not.
    /// </summary>
    /// 
    /// <exception cref="MilvaUserFriendlyException"> Throwns when <paramref name="id"/> is defult record id. </exception>
    /// 
    /// <param name="id"></param>
    public static void CheckContentIsDefaultRecord(this int id)
    {
        if (id is > GlobalConstant.Zero and < 50) throw new MilvaUserFriendlyException(MilvaException.CannotUpdateOrDeleteDefaultRecord);
    }

    /// <summary>
    /// Checks <paramref name="idList"/> contains default record or not.
    /// </summary>
    /// 
    /// <exception cref="MilvaUserFriendlyException"> Throwns when contents contains defult record id. </exception>
    /// 
    /// <param name="idList"></param>
    public static void CheckContentIsDefaultRecord(this List<int> idList)
    {
        if (idList.Any(i => i is > GlobalConstant.Zero and < 50)) throw new MilvaUserFriendlyException(MilvaException.CannotUpdateOrDeleteDefaultRecord);
    }

    /// <summary>
    /// Checks <paramref name="id"/> is default record id or not.
    /// </summary>
    /// <param name="id"></param>
    public static bool IsDefaultRecord(this int id)
    {
        return id is > GlobalConstant.Zero and < 50;
    }

    #endregion
}
