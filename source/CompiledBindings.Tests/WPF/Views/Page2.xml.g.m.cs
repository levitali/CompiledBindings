namespace WPFTest.Views
{
#pragma warning disable 8600
#pragma warning disable 8603
#pragma warning disable 8618
#pragma warning disable 8625

	[System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page2
	{
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;


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

			public void Initialize(Page2 dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

				_targetRoot = dataRoot;

				Update();
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
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
				targetRoot.textBlock1.Text = dataRoot.Prop1;

			}
		}
	}
}
