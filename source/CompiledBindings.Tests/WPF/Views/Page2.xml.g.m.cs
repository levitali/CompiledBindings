namespace WPFTest.Views
{
#nullable disable

	[System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page2
	{
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;

#line (17, 17) - (17, 40) 17 "Page2.xml"
			button2.Click += (p1, p2) => this.OnClick2();
#line default

			Bindings_.Initialize(this);
		}

		private void DeinitializeAfterDestructor()
		{
			if (Bindings_ != null)
			{
				Bindings_.Cleanup();
			}
		}

		Page2_Bindings_ Bindings_ = new Page2_Bindings_();

		class Page2_Bindings_
		{
			Page2 _targetRoot;
			global::System.Windows.RoutedEventHandler _eventHandler2;
			global::System.Threading.CancellationTokenSource _generatedCodeDisposed;

			public void Initialize(Page2 dataRoot)
			{
				_targetRoot = dataRoot;
				_generatedCodeDisposed = new global::System.Threading.CancellationTokenSource();

				Update();

#line (16, 17) - (16, 41) 16 "Page2.xml"
				_eventHandler2 = dataRoot.OnClick1;
#line default
				_targetRoot.button1.Click += _eventHandler2;
#line default
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_generatedCodeDisposed.Cancel();
					_targetRoot.button1.Click -= _eventHandler2;
					_eventHandler2 = null;
					_targetRoot = null;
				}
			}

			public void Update()
			{
				var dataRoot = _targetRoot;
#line (14, 20) - (14, 40) 14 "Page2.xml"
				_targetRoot.textBlock1.Text = dataRoot.Prop1;
#line default
				Set0(_generatedCodeDisposed.Token);
				async void Set0(global::System.Threading.CancellationToken cancellationToken)
				{
					try
					{
#line (15, 20) - (15, 40) 15 "Page2.xml"
						var result = await dataRoot.Prop2;
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							_targetRoot.textBlock2.Text = result;
						}
					}
					catch
					{
					}
				}
#line default
			}
		}
	}
}
