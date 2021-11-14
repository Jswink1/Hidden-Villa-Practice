using Blazored.LocalStorage;
using HiddenVilla_ClientPractice.Helper;
using HiddenVilla_Models;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiddenVilla_ClientPractice.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public void NotifyUserLogin(string token)
        {
            // Get authentication state from user
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token),
                                                                                   "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            // Notify that user has logged in
            NotifyAuthenticationStateChanged(authState);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Load Token from Local Storage
            var token = await _localStorage.GetItemAsync<string>(SD.Local_Token);

            // If there is no token, there is no logged in user
            if (token == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Set auth header bearer token, return Auth State
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
        }

        public void NotifyUserLogout()
        {
            // Get authentication state
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

            // Notify that user has logged out
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
