using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using AllReady.Security;
using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Http;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using System.Threading.Tasks;
using AllReady.Services;
using AllReady.Extensions;
using System;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class CampaignController : Controller
    {
        private readonly IMediator _bus;
        private readonly IImageService _imageService;

        public CampaignController(IMediator bus, IImageService imageService)
        {
            _bus = bus;
            _imageService = imageService;
        }

        // GET: Campaign
        public IActionResult Index()
        {
            var query = new CampaignListQuery();
            if (User.IsUserType(UserType.OrgAdmin))
            {
                query.OrganizationId = User.GetOrganizationId();
            }
            var campaigns = _bus.Send(query);
            return View(campaigns);
        }

        public IActionResult Details(int id)
        {
            CampaignDetailModel campaign = _bus.Send(new CampaignDetailQuery { CampaignId = id });

            if (campaign == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(campaign);
        }

        // GET: Campaign/Create
        public IActionResult Create()
        {
            return View("Edit", new CampaignSummaryModel()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            });
        }

        // GET: Campaign/Edit/5
        public IActionResult Edit(int id)
        {
            CampaignSummaryModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = id });

            if (campaign == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(campaign);
        }

        // POST: Campaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CampaignSummaryModel campaign, IFormFile fileUpload)
        {
            if (campaign == null)
            {
                return HttpBadRequest();
            }

            if (!User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return HttpUnauthorized();
            }

            // Tempoary code to avoid current database update error when the postcodegeo does not exist in the database.
            if (campaign.Location != null && !string.IsNullOrEmpty(campaign.Location.PostalCode))
            {
                bool validPostcode = await _bus.SendAsync(new CheckValidPostcodeQueryAsync
                {
                    Postcode = new PostalCodeGeo
                    {
                        City = campaign.Location.City,
                        State = campaign.Location.State,
                        PostalCode = campaign.Location.PostalCode
                    }
                });

                if (!validPostcode)
                {
                    ModelState.AddModelError(campaign.Location.PostalCode, "The city, state and postal code combination is not valid");
                }
            }

            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    if (fileUpload.IsAcceptableImageContentType())
                    {
                        campaign.ImageUrl = await _imageService.UploadCampaignImageAsync(campaign.OrganizationId, campaign.Id, fileUpload);
                    }
                    else
                    {
                        ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View(campaign);
                    }
                }

                int id = _bus.Send(new EditCampaignCommand { Campaign = campaign });
                return RedirectToAction("Details", new { area = "Admin", id = id });
            }
            return View(campaign);
        }

        // GET: Campaign/Delete/5
        public IActionResult Delete(int id)
        {
            CampaignSummaryModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = id });

            if (campaign == null)
            {
                return HttpNotFound();
            }
            if (!User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(campaign);
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            CampaignSummaryModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = id });

            if (!User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return HttpUnauthorized();
            }

            _bus.Send(new DeleteCampaignCommand { CampaignId = id });
            return RedirectToAction("Index", new { area = "Admin" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LockUnlock(int id)
        {
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                return HttpUnauthorized();
            }

            _bus.Send(new LockUnlockCampaignCommand { CampaignId = id });
            return RedirectToAction("Details", new { area = "Admin", id = id });
        }
    }
}