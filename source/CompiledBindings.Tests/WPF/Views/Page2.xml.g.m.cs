namespace WPFTest.Views
{
	using System.Threading;
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

			button2.Click += (p1, p2) => this.OnClick2();

			Bindings_.Initialize(this);
		}

		~Page2()
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
			CancellationTokenSource _generatedCodeDisposed;

			public void Initialize(Page2 dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

				_targetRoot = dataRoot;
				_generatedCodeDisposed = new CancellationTokenSource();

				Update();

				_eventHandler2 = dataRoot.OnClick1;
				_targetRoot.button1.Click += _eventHandler2;
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
				if (_targetRoot == null)
				{
					throw new System.InvalidOperationException();
				}

				var targetRoot = _targetRoot;
				var dataRoot = _targetRoot;
				var bindings = this;
				targetRoot.textBlock1.Text = dataRoot.Prop1;
				Set0(bindings._generatedCodeDisposed.Token);
				async void Set0(CancellationToken cancellationToken)
				{
					try
					{
						var value = await dataRoot.Prop2;
						if (!cancellationToken.IsCancellationRequested)
						{
							targetRoot.textBlock2.Text = value;
						}
					}
					catch
					{
					}
				}

			}
		}
	}
}
