#pragma once
#include <ppltasks.h>

using namespace Windows::UI::Xaml::Controls;
using namespace Microsoft::Graphics::Canvas::Brushes;
using namespace Microsoft::Graphics::Canvas;
using namespace concurrency;

namespace CustomControlNative
{
	[Windows::Foundation::Metadata::WebHostHidden()]
	public ref class ImageEx sealed : public Control
	{
	public:
		ImageEx();
		virtual ~ImageEx();
		void Invalidate();

		property Platform::String^ FilePath
		{
			Platform::String^ get()
			{
				return m_filePath;
			}
			void set(Platform::String^ value)
			{
				m_filePath = value;
			}
		}

		property Windows::Storage::Streams::IRandomAccessStream^ FileStream
		{
			Windows::Storage::Streams::IRandomAccessStream^ get()
			{
				return m_fileStream;
			}
			void set(Windows::Storage::Streams::IRandomAccessStream^ value)
			{
				m_fileStream = value;
			}
		}

	private:
		Microsoft::Graphics::Canvas::UI::Xaml::CanvasControl^ m_canvasControl;
		Windows::UI::Xaml::Controls::Grid^ m_rootGrid;

		Platform::String^ m_filePath;
		Windows::Storage::Streams::IRandomAccessStream^ m_fileStream;
		Microsoft::Graphics::Canvas::CanvasBitmap^ m_bitmap;

		Windows::Foundation::IAsyncAction^ CreateResourceAsync(Microsoft::Graphics::Canvas::UI::Xaml::CanvasControl^ canvas);

	protected:
		void OnApplyTemplate() override;
		void OnDraw(Microsoft::Graphics::Canvas::UI::Xaml::CanvasControl ^sender, Microsoft::Graphics::Canvas::UI::Xaml::CanvasDrawEventArgs ^args);
		void OnLoaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e);
		void OnCreateResources(Microsoft::Graphics::Canvas::UI::Xaml::CanvasControl ^sender, Microsoft::Graphics::Canvas::UI::CanvasCreateResourcesEventArgs ^args);
	};
}


