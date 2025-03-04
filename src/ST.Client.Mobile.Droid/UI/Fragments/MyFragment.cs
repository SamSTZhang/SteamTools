using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Binding;
using ReactiveUI;
using System.Application.Mvvm;
using System.Application.Services;
using System.Application.UI.Activities;
using System.Application.UI.Adapters;
using System.Application.UI.ViewModels;
using System.Collections.Generic;
using System.Reactive.Disposables;
using static System.Application.UI.ViewModels.MyPageViewModel;

namespace System.Application.UI.Fragments
{
    [Register(JavaPackageConstants.Fragments + nameof(MyFragment))]
    internal sealed class MyFragment : BaseFragment<fragment_my, MyPageViewModel>, IDisposableHolder
    {
        readonly CompositeDisposable disposables = new();
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => disposables;

        protected override int? LayoutResource => Resource.Layout.fragment_my;

        public override void OnCreateView(View view)
        {
            base.OnCreateView(view);

            ViewModel!.WhenAnyValue(x => x.NickName).Subscribe(value =>
            {
                if (binding == null) return;
                binding.tvNickName.Text = value;
            }).AddTo(this);

            SetOnClickListener(binding!.layoutUser);

            var adapter = new MyPreferenceButtonAdapter(ViewModel!);
            adapter.ItemClick += (_, e) =>
            {
                if (e.Current.Authentication && !UserService.Current.IsAuthenticated)
                {
                    OnClick(binding!.layoutUser);
                    return;
                }

                var activityType = e.Current.Id switch
                {
                    PreferenceButton.EditProfile => typeof(EditProfileActivity),
                    PreferenceButton.BindPhoneNumber => typeof(BindPhoneNumberActivity),
                    PreferenceButton.ChangePhoneNumber => typeof(ChangePhoneNumberActivity),
                    PreferenceButton.Settings => typeof(SettingsActivity),
                    PreferenceButton.About => typeof(AboutActivity),
                    _ => (Type?)null,
                };
                if (activityType != null) this.StartActivity(activityType);
            };
            var layout = new LinearLayoutManager(Context, LinearLayoutManager.Vertical, false);
            binding.rvPreferenceButtons.SetLayoutManager(layout);
            binding.rvPreferenceButtons.AddItemDecoration(new VerticalItemViewGroupDecoration(binding.rvPreferenceButtons.PaddingTop));
            binding.rvPreferenceButtons.SetAdapter(adapter);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            disposables.Dispose();
        }

        protected override bool OnClick(View view)
        {
            if (view.Id == Resource.Id.layoutUser)
            {
                this.StartActivity<LoginOrRegisterActivity>();
            }
            //else if (view.Id == Resource.Id.???)
            //{
            //}

            return base.OnClick(view);
        }
    }
}