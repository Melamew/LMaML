
namespace iLynx.Common
{
	/// <summary>
	/// Simple Interface that can be implemented to "validate" settings or other values
	/// </summary>
    public interface IValidatable
    {
		/// <summary>
		/// Validates whatever the implementor chose to validate
		/// </summary>
		/// <returns>Returns true if the values are valid, otherwise false</returns>
        bool Validate();
    }
}
