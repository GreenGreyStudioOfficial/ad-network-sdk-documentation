using System.Collections.Generic;
using GreenGrey.AdNetworkSDK.DataModel.Enums;
using GreenGrey.AdNetworkSDK.Interfaces.Connector;
using GreenGrey.AdNetworkSDK.Interfaces.Listeners.Initialization;
using GreenGrey.AdNetworkSDK.Interfaces.Listeners.Load;
using GreenGrey.AdNetworkSDK.Interfaces.Listeners.Show;
using UnityEngine;

namespace GGADSDK.Samples.ApplovinExample.Scripts.Connectors.Applovin
{
    /// <summary>
    /// This is example implementation of Applovin connector.
    /// You can modify this one or create your own.
    /// Your realisation MUST implement ISdkConnector interface
    /// </summary>
    public class ApplovinConnector: ISdkConnector
    {
        /// <summary>
        /// Айди applovin
        /// </summary>
        private const string APPLOVIN = "applovin";
        
        /// <summary>
        /// Applovin sdk key
        /// </summary>
        private string m_sdkKey;
        /// <summary>
        /// Add userId if you want detect revenue by users
        /// </summary>
        private string m_userId;
        /// <summary>
        /// Reward indicator.
        /// If true user should receive reward
        /// </summary>
        private bool m_rewardReceived;
        /// <summary>
        /// Initialize listener implementation
        /// </summary>
        private IAdInitializationListener m_initializationListener;
        /// <summary>
        /// List of existed AdType`s supported by your load implementation of ISDKConnector.Load
        /// </summary>
        private readonly List<AdType> m_supportedAdTypes;

        /// <summary>
        /// Dictionary of load listeners for ads;
        /// Key - placementId, Value - loadListener
        /// </summary>
        private readonly Dictionary<string, IAdLoadListener> m_loadListeners;
        /// <summary>
        /// Dictionary of show listeners for  ads;
        /// key - placementId, Value - showListener
        /// </summary>
        private readonly Dictionary<string, IAdShowListener> m_showListeners;

        public ApplovinConnector(string _sdkKey, string _userId = null)
        {
            m_sdkKey = _sdkKey;
            m_userId = _userId;
            isInitialized = false;
            isInitializeStarted = false;
            // include only supported ad types by you Load
            m_supportedAdTypes = new List<AdType>
            {
                AdType.REWARDED,
                AdType.INTERSTITIAL,
                AdType.BANNER,
                AdType.MREC
            };

            m_loadListeners = new Dictionary<string, IAdLoadListener>();
            m_showListeners = new Dictionary<string, IAdShowListener>();
        }

        #region ISDKConnector
        
        public bool isInitialized { get; private set; }
        public bool isInitializeStarted { get; private set; }

        public void Initialize(IAdInitializationListener _listener)
        {
            isInitializeStarted = true;
            m_initializationListener = _listener;
            
            // you can add more callback listeners supported by applovin
            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitialized;
            
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
            
            MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
            
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            
            MaxSdk.SetSdkKey(m_sdkKey);
            MaxSdk.SetUserId(m_userId);
            MaxSdk.InitializeSdk();
        }

        public void Load(AdType _adType, IAdLoadListener _listener, string _placementId)
        {
            if (_placementId == null)
            {
                _listener.OnLoadError(_adType, LoadErrorType.THIRD_PARTY_CONNECTOR_ERROR,
                    "PlacementId required. Get AD_UNIT_ID in your applovin");
                return;
            }

            m_loadListeners[_placementId] = _listener;
            switch (_adType)
            {
                case AdType.INTERSTITIAL:
                    LoadInterstitial(_placementId);
                    break;
                case AdType.REWARDED:
                    LoadRewardedAd(_placementId);
                    break;
                case AdType.BANNER:
                    LoadBannerAd(_placementId);
                    break;
                case AdType.MREC:
                    LoadMrec(_placementId);
                    break;
                default:
                    m_loadListeners.Remove(_placementId);
                    break;
            }
        }

        public void Show(AdType _adType, IAdShowListener _listener, string _placementId)
        {
            if (_placementId == null)
            {
                _listener.OnShowError(_adType, ShowErrorType.THIRD_PARTY_CONNECTOR_ERROR,
                    "PlacementId required. Get AD_UNIT_ID in your applovin");
                return;
            }
            
            m_showListeners[_placementId] = _listener;
            switch (_adType)
            {
                case AdType.INTERSTITIAL:
                    ShowInterstitial(_placementId);
                    break;
                case AdType.REWARDED:
                    ShowRewarded(_placementId);
                    break;
                case AdType.BANNER:
                    ShowBanner(_placementId);
                    break;
                case AdType.MREC:
                    ShowMREC(_placementId);
                    break;
                default:
                    m_showListeners.Remove(_placementId);
                    break;
            }
        }

        public List<AdType> GetSupportedAdTypes()
        {
            return m_supportedAdTypes;
        }

        public string GetSdkId()
        {
            return APPLOVIN;
        }

        #endregion

        #region Applovin Initialization

        private void OnSdkInitialized(MaxSdkBase.SdkConfiguration _sdkConfiguration)
        {
            if (_sdkConfiguration.IsSuccessfullyInitialized)
            {
                m_initializationListener.OnInitializationComplete();
                isInitialized = true;
                isInitializeStarted = false;
                Debug.Log("Applovin successfully initialized");
            }
            else
            {
                m_initializationListener.OnInitializationError(InitializationErrorType.THIRD_PARTY_CONNECTOR_ERROR, "cant initialize applovin sdk");
                isInitialized = false;
                isInitializeStarted = false;
                
                MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitialized;
            
                MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialLoadedEvent;
                MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialLoadFailedEvent;
            
                MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoadedEvent;
                MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailedEvent;
            
                MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnBannerAdLoadedEvent;
                MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnBannerAdLoadFailedEvent;
            
                MaxSdkCallbacks.MRec.OnAdLoadedEvent -= OnMRecAdLoadedEvent;
                MaxSdkCallbacks.MRec.OnAdLoadFailedEvent -= OnMRecAdLoadFailedEvent;
            
                MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialDisplayedEvent;
                MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialHiddenEvent;
                MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnInterstitialAdFailedToDisplayEvent;
            
                MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedDisplayedEvent;
                MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedHiddenEvent;
                MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
                MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
                
                Debug.Log("Applovin initialization failed");
            }
        }

        #endregion

        #region Applovin Load

        #region Interstitial

        private void LoadInterstitial(string _placementId)
        {
            MaxSdk.LoadInterstitial(_placementId);
        }
        
        private void OnInterstitialLoadedEvent(string _placementId, MaxSdkBase.AdInfo _adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
            if (m_loadListeners.ContainsKey(_placementId))
                m_loadListeners[_placementId].OnLoadComplete(AdType.INTERSTITIAL);
        }
        
        private void OnInterstitialLoadFailedEvent(string _placementId, MaxSdkBase.ErrorInfo _errorInfo)
        {
            // Interstitial ad failed to load 
            if (m_loadListeners.ContainsKey(_placementId))
                m_loadListeners[_placementId].OnLoadError(AdType.INTERSTITIAL, LoadErrorType.UNKNOWN, _errorInfo.Message);
        }

        #endregion

        #region Rewarded

        private void LoadRewardedAd(string _placementId)
        {
            MaxSdk.LoadRewardedAd(_placementId);
        }

        private void OnRewardedAdLoadedEvent(string _placementId, MaxSdkBase.AdInfo _adInfo)
        {
            if (m_loadListeners.ContainsKey(_placementId))
                m_loadListeners[_placementId].OnLoadComplete(AdType.REWARDED);
        }
        
        private void OnRewardedAdLoadFailedEvent(string _placementId, MaxSdkBase.ErrorInfo _errorInfo)
        {
            // Rewarded ad failed to load 
            if (m_loadListeners.ContainsKey(_placementId))
                m_loadListeners[_placementId].OnLoadError(AdType.REWARDED, LoadErrorType.UNKNOWN, _errorInfo.Message);
        }

        #endregion

        #region Banner

        private void LoadBannerAd(string _placementId)
        {
            MaxSdk.CreateBanner(_placementId, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.SetBannerBackgroundColor(_placementId, Color.yellow);
        }

        private void OnBannerAdLoadedEvent(string _placementId, MaxSdkBase.AdInfo _adInfo)
        {
            if (m_loadListeners.ContainsKey(_placementId))
                m_loadListeners[_placementId].OnLoadComplete(AdType.BANNER);
        }
        
        private void OnBannerAdLoadFailedEvent(string _placementId, MaxSdkBase.ErrorInfo _errorInfo)
        {
            if (m_loadListeners.ContainsKey(_placementId))
                m_loadListeners[_placementId].OnLoadError(AdType.BANNER, LoadErrorType.UNKNOWN, _errorInfo.Message);
        }

        #endregion

        #region MREC

        private void LoadMrec(string _placementId)
        {
            MaxSdk.CreateMRec(_placementId, MaxSdkBase.AdViewPosition.Centered);
        }

        private void OnMRecAdLoadedEvent(string _placementId, MaxSdkBase.AdInfo _adInfo)
        {
            if (m_loadListeners.ContainsKey(_placementId))
                m_loadListeners[_placementId].OnLoadComplete(AdType.MREC);
        }
        
        private void OnMRecAdLoadFailedEvent(string _placementId, MaxSdkBase.ErrorInfo _error)
        {
            if (m_loadListeners.ContainsKey(_placementId))
                m_loadListeners[_placementId].OnLoadError(AdType.MREC, LoadErrorType.UNKNOWN, _error.Message);
        }

        #endregion

        #endregion

        #region Applovin Show

        #region Interstitial

        private void ShowInterstitial(string _placementId)
        {
            if (!MaxSdk.IsInterstitialReady(_placementId) && m_showListeners.ContainsKey(_placementId))
            {
                m_showListeners[_placementId].OnShowError(
                    AdType.INTERSTITIAL, ShowErrorType.NO_LOADED_CONTENT, ShowErrorType.NO_LOADED_CONTENT.ToString());
                return;
            }
            MaxSdk.ShowInterstitial(_placementId);
        }

        private void OnInterstitialDisplayedEvent(string _placementId, MaxSdkBase.AdInfo _adInfo)
        {
            if (m_showListeners.ContainsKey(_placementId))
                m_showListeners[_placementId].OnShowStart(AdType.INTERSTITIAL);
        }
        
        private void OnInterstitialAdFailedToDisplayEvent(string _placementId, MaxSdkBase.ErrorInfo _errorInfo, MaxSdkBase.AdInfo _adInfo)
        {
            if (m_showListeners.ContainsKey(_placementId))
                m_showListeners[_placementId]
                    .OnShowError(AdType.INTERSTITIAL, ShowErrorType.UNKNOWN, _errorInfo.Message);
        }
        
        private void OnInterstitialHiddenEvent(string _placementId, MaxSdkBase.AdInfo _adInfo)
        {
            var closeStatus = ShowCompletionState.SHOW_COMPLETE_BY_CLOSE_BUTTON;
            if (m_showListeners.ContainsKey(_placementId))
                m_showListeners[_placementId].OnShowComplete(AdType.INTERSTITIAL, closeStatus, null);
        }

        #endregion
        
        #region Rewarded

        private void ShowRewarded(string _placementId)
        {
            if (!MaxSdk.IsRewardedAdReady(_placementId) && m_showListeners.ContainsKey(_placementId))
            {
                m_showListeners[_placementId].OnShowError(
                    AdType.REWARDED, ShowErrorType.NO_LOADED_CONTENT, ShowErrorType.NO_LOADED_CONTENT.ToString());
                return;
            }
            MaxSdk.ShowRewardedAd(_placementId);
            m_rewardReceived = false;
        }

        private void OnRewardedDisplayedEvent(string _placementId, MaxSdkBase.AdInfo _adInfo)
        {
            if (m_showListeners.ContainsKey(_placementId))
                m_showListeners[_placementId].OnShowStart(AdType.REWARDED);
        }
        
        private void OnRewardedAdFailedToDisplayEvent(string _placementId, MaxSdkBase.ErrorInfo _errorInfo, MaxSdkBase.AdInfo _adInfo)
        {
            if (m_showListeners.ContainsKey(_placementId))
                m_showListeners[_placementId]
                    .OnShowError(AdType.REWARDED, ShowErrorType.UNKNOWN, _errorInfo.Message);
        }
        
        private void OnRewardedHiddenEvent(string _placementId, MaxSdkBase.AdInfo _adInfo)
        {
            var closeStatus = m_rewardReceived
                ? ShowCompletionState.SHOW_COMPLETE_BY_CLOSE_BUTTON
                : ShowCompletionState.SHOW_COMPLETE_BY_SKIP_BUTTON;
            if (m_showListeners.ContainsKey(_placementId))
                m_showListeners[_placementId].OnShowComplete(AdType.REWARDED, closeStatus, null);
        }
        
        private void OnRewardedAdReceivedRewardEvent(string _placementId, MaxSdk.Reward _reward, MaxSdkBase.AdInfo _adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            m_rewardReceived = true;
        }

        #endregion

        #region Banner

        private void ShowBanner(string _placementId)
        {
            MaxSdk.ShowBanner(_placementId);
        }

        #endregion

        #region MREC
        
        private void ShowMREC(string _placementId)
        {
            MaxSdk.ShowMRec(_placementId);
        }

        #endregion
        
        #endregion
    }
}
