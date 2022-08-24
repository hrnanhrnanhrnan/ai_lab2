using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ai_lab2_webapp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IComputerVisionClient _client;

        // injects the ComputerVisionClient to the IndexModel
        public IndexModel(ILogger<IndexModel> logger, IComputerVisionClient client)
        {
            _logger = logger;
            _client = client;
        }

        [BindProperty]
        public ImageAnalysis ImageAnalyzisResult { get; set; }

        [BindProperty(SupportsGet = true)]
        [MaxLength(200, ErrorMessage = "URL max length is 200 characters")]
        public string ImageURL { get; set; }

        public async Task<IActionResult> OnGet()
        {
            ViewData["Title"] = "Analyzer";
            // if the imageurl is not null or empty create a new list of visualfeaturetypes
            // and pass that along with the imageurl to the analyzeimage method
            // and the result of that method is set to the ImageAnalyzisResult
            if (!String.IsNullOrEmpty(ImageURL))
            {
                var features = new List<VisualFeatureTypes?>()
                {
                    VisualFeatureTypes.Brands,
                    VisualFeatureTypes.Categories,
                    VisualFeatureTypes.Color,
                    VisualFeatureTypes.Description,
                    VisualFeatureTypes.Objects,
                    VisualFeatureTypes.Tags
                };
                try
                {
                    ImageAnalyzisResult = await _client.AnalyzeImageAsync(ImageURL, visualFeatures: features);
                }
                catch (Exception e)
                {
                    return RedirectToPage("/Error", new { e.Message });
                }
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            // if the modelstate is not valid then set the imageurl to null and return the index page
            // else redirect to index page with the valid imageurl as a routevalue
            if (!ModelState.IsValid)
            {
                ImageURL = null;
                return Page();
            }
            return RedirectToPage("/Index", new { ImageURL });
        }
    }
}
