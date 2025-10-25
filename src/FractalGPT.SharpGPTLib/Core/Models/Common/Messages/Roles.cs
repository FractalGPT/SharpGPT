namespace FractalGPT.SharpGPTLib.Core.Models.Common.Messages;

/// <summary>
/// Roles for chat messages.
/// </summary>
[Serializable]
public enum Roles : byte
{
    Assistant = 1,
    User = 2,
    System = 3
}