using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebAppAcademics.Client.Services;
using WebAppAcademics.Client.Shared;
using WebAppAcademics.Shared.Models.Settings;

namespace WebAppAcademics.Client.Pages.Admin.Staff
{
    public partial class CountryList
    {
        #region [Injection Declaration]
        [Inject] NavigationManager navManager { get; set; }
        [Inject] SweetAlertService Swal { get; set; }
        [Inject] IAPIServices<SETCountries> countryService { get; set; }
        [Inject] IAPIServices<SETStates> stateService { get; set; }
        [Inject] IAPIServices<SETLGA> lgaService { get; set; }


        [CascadingParameter]
        public MainLayout Layout { get; set; }
        #endregion

        #region [Variables Declaration]
        int toolBarMenuId { get; set; }
        int selectedCountryID { get; set; }
        int selectedStatdID { get; set; }
        string selectedCountry { get; set; }
        string selectedState { get; set; }

        #endregion

        #region [Models Declaration]
        List<SETCountries> countries { get; set; }
        List<SETStates> states { get; set; }
        List<SETLGA> lgas { get; set; }

        SETCountries countrydetails = new SETCountries();
        SETStates statedetails = new SETStates();
        SETLGA lgadetails = new SETLGA();

      
        void InitializeModels()
        {
            countries = new List<SETCountries>();
            states = new List<SETStates>();
            lgas = new List<SETLGA>();
        }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            InitializeModels();
            Layout.currentPage = "Country List";
            toolBarMenuId = 1;
            countries = await countryService.GetAllAsync("Settings/GetCountries/1");
            await base.OnInitializedAsync();
        }

        #region [Section - Country]
        void SelectedCountryRow(TableRowClickEventArgs<SETCountries> model)
        {
            selectedCountryID = model.Item.CountryID;
            countrydetails.CountryID = model.Item.CountryID;
            countrydetails.CountryCode = model.Item.CountryCode;
            countrydetails.Country = model.Item.Country;
        }

        async Task SubmitValidFormCountry()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Country Save/Update Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                if (countrydetails.CountryID == 0)
                {
                    var response = await countryService.SaveAsync("Settings/AddCountry/", countrydetails);
                    countrydetails.CountryID = response.CountryID;
                    countrydetails.Id = response.CountryID;
                    await countryService.UpdateAsync("Settings/UpdateCountry/", 2, countrydetails);
                    await Swal.FireAsync("New Country", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    await countryService.UpdateAsync("Settings/UpdateCountry/", 1, countrydetails);
                    await Swal.FireAsync("Selected Country", "Has Been Successfully Updated.", "success");
                }

                await CountryEvent();
            }
        }

        void CountryReset()
        {
            countrydetails.CountryID = 0;
            countrydetails.CountryCode = string.Empty;
            countrydetails.Country = string.Empty;
        }

        #endregion

        #region [Section - States]
        async Task OnCountryChanged(IEnumerable<string> value)
        {
            selectedCountry = value.ElementAt(0);
            statedetails.CountryID = countries.FirstOrDefault(s => s.Country == selectedCountry).CountryID;
            statedetails.Country = value.ElementAt(0);

            states = await stateService.GetAllAsync("Settings/GetStates/1/" + statedetails.CountryID);
        }

        void SelectedStateRow(TableRowClickEventArgs<SETStates> model)
        {
            selectedStatdID = model.Item.StateID;
            statedetails.StateID = model.Item.StateID;
            statedetails.CountryID = model.Item.CountryID;
            statedetails.Country = model.Item.Country;
            statedetails.StateCode = model.Item.StateCode;
            statedetails.State = model.Item.State;
        }

        async Task SubmitValidFormState()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "State Save/Update Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                if (statedetails.StateID == 0)
                {
                    var response = await stateService.SaveAsync("Settings/AddState/", statedetails);
                    statedetails.StateID = response.StateID;
                    statedetails.Id = response.StateID;
                    await stateService.UpdateAsync("Settings/UpdateState/", 2, statedetails);
                    await Swal.FireAsync("New State", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    await stateService.UpdateAsync("Settings/UpdateState/", 1, statedetails);
                    await Swal.FireAsync("Selected State", "Has Been Successfully Updated.", "success");
                }

                StateReset();
                states = await stateService.GetAllAsync("Settings/GetStates/1/" + statedetails.CountryID);
            }
        }

        void StateReset()
        {
            statedetails.StateID = 0;
            statedetails.StateCode = string.Empty;
            statedetails.State = string.Empty;
        }


        #endregion

        #region [Section - Local Government]
        async Task OnCountryChangedForLGA(IEnumerable<string> value)
        {
            selectedCountry = value.ElementAt(0);
            statedetails.CountryID = countries.FirstOrDefault(s => s.Country == selectedCountry).CountryID;
            statedetails.Country = value.ElementAt(0);

            states = await stateService.GetAllAsync("Settings/GetStates/1/" + statedetails.CountryID);
        }

        async Task OnStateChanged(IEnumerable<string> value)
        {
            selectedState = value.ElementAt(0);
            lgadetails.StateID = states.FirstOrDefault(s => s.State == selectedState).StateID;
            lgadetails.State = value.ElementAt(0);

            lgas = await lgaService.GetAllAsync("Settings/GetLGAs/1/" + lgadetails.StateID);
        }

        void SelectedLGARow(TableRowClickEventArgs<SETLGA> model)
        {
            lgadetails.LGAID = model.Item.LGAID;
            lgadetails.StateID = model.Item.StateID;
            lgadetails.State = model.Item.State;
            lgadetails.LGACode = model.Item.LGACode;
            lgadetails.LGA = model.Item.LGA;
        }

        async Task SubmitValidFormLGA()
        {
            SweetAlertResult result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "LGA Save/Update Operation",
                Text = "Do You Want To Continue With This Operation?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, Contnue!",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                if (lgadetails.LGAID == 0)
                {
                    var response = await lgaService.SaveAsync("Settings/AddLGA/", lgadetails);
                    lgadetails.LGAID = response.LGAID;
                    lgadetails.Id = response.LGAID;
                    await lgaService.UpdateAsync("Settings/UpdateLGA/", 2, lgadetails);
                    await Swal.FireAsync("New LGA", "Has Been Successfully Saved.", "success");
                }
                else
                {
                    await lgaService.UpdateAsync("Settings/UpdateLGA/", 1, lgadetails);
                    await Swal.FireAsync("Selected LGA", "Has Been Successfully Updated.", "success");
                }

                LGAReset();
                lgas = await lgaService.GetAllAsync("Settings/GetLGAs/1/" + lgadetails.StateID);
            }
        }

        void LGAReset()
        {
            lgadetails.LGAID = 0;
            lgadetails.LGACode = string.Empty;
            lgadetails.LGA = string.Empty;
        }


        #endregion

        #region [Section - Click Events]
        async Task CountryEvent()
        {
            toolBarMenuId = 1;
            countries = await countryService.GetAllAsync("Settings/GetCountries/1");
            CountryReset();
        }

        async Task StateEvent()
        {
            toolBarMenuId = 2;
            selectedCountry = string.Empty;
            countries.Clear();
            countries = await countryService.GetAllAsync("Settings/GetCountries/1");
            states.Clear();
            StateReset();
        }

        async Task LGAEvent()
        {
            toolBarMenuId = 3;
            selectedCountry = string.Empty;
            countries.Clear();
            countries = await countryService.GetAllAsync("Settings/GetCountries/1");
            selectedState = string.Empty;
            states.Clear();
            lgas.Clear();
        }

        async Task InvalidEntries()
        {
            await Swal.FireAsync("Please Check Your Entries.", "One or More Entries has error.", "error");
        }

        void GoBack()
        {
            navManager.NavigateTo("/staffs");
        }
        #endregion

    }
}
