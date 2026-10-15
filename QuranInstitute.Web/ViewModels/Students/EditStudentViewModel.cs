using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuranInstitute.Web.ViewModels.Students;

public class EditStudentViewModel : BaseStudentFormViewModel
{
    public Guid Id { get; set; }

 
}