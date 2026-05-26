using System;

namespace Sample.Namespace;

/// <summary>
/// 表示一組具有分層入口的選項
/// </summary>
public readonly struct LayeredChoice : IEquatable<LayeredChoice>
{
	/// <summary>
	/// 內部值
	/// </summary>
	private enum InnerValue : byte
	{
		/// <summary>預設路徑</summary>
		DefaultPath = 0,

		/// <summary>替代路徑</summary>
		AlternatePath = 1,

		/// <summary>需明確選擇的路徑</summary>
		SpecialPath = 2
	}

	/// <summary>
	/// 目前值
	/// </summary>
	private readonly InnerValue _value = default;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="value">內部值</param>
	private LayeredChoice(InnerValue value) => _value = value;

	/// <summary>
	/// 預設路徑
	/// </summary>
	public static LayeredChoice DefaultPath => new(InnerValue.DefaultPath);

	/// <summary>
	/// 替代路徑
	/// </summary>
	public static LayeredChoice AlternatePath => new(InnerValue.AlternatePath);

	/// <summary>
	/// 需明確選擇的選項群組
	/// </summary>
	public static class Gated
	{
		/// <summary>
		/// 需明確選擇的路徑
		/// </summary>
		public static LayeredChoice SpecialPath => new(InnerValue.SpecialPath);
	}

	/// <summary>
	/// 是否為需明確選擇的路徑
	/// </summary>
	public bool IsSpecialPath => _value == InnerValue.SpecialPath;

	/// <inheritdoc />
	public bool Equals(LayeredChoice other) => _value == other._value;

	/// <inheritdoc />
	public override bool Equals(object? obj) => obj is LayeredChoice other && Equals(other);

	/// <inheritdoc />
	public override int GetHashCode() => (byte)_value;

	/// <inheritdoc />
	public static bool operator ==(LayeredChoice left, LayeredChoice right) => left.Equals(right);

	/// <inheritdoc />
	public static bool operator !=(LayeredChoice left, LayeredChoice right) => !left.Equals(right);
}