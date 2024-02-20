using System.ComponentModel.DataAnnotations;

namespace CitiesManager.WebAPI.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class City
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid CityID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "City Name can't be blank")]
        public string? CityName { get; set; }
    }
}
