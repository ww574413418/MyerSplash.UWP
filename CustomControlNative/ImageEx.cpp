#include "pch.h"
#include "ImageEx.h"
#include <ppltasks.h>

using namespace CustomControlNative;
using namespace Windows::UI::Xaml::Controls;
using namespace Microsoft::Graphics::Canvas::UI::Xaml;
using namespace Microsoft::Graphics::Canvas;
using namespace Windows::Foundation;
using namespace concurrency;
using namespace Platform;
using namespace std;

ImageEx::ImageEx()
{
	DefaultStyleKey = "CustomControlNative.ImageEx";
	this->Loaded += ref new Windows::UI::Xaml::RoutedEventHandler(this, &CustomControlNative::ImageEx::OnLoaded);
}

void ImageEx::Invalidate()
{
	if (m_canvasControl != nullptr)
	{
		m_canvasControl->Invalidate();
	}
}

ImageEx::~ImageEx()
{

}

Windows::Foundation::IAsyncAction^ CustomControlNative::ImageEx::CreateResourceAsync(CanvasControl^ canvas)
{
	return create_async([=]()
	{
		return create_task(CanvasBitmap::LoadAsync(canvas, FilePath))
			.then([=](CanvasBitmap^ bitmap)
		{
			m_bitmap = bitmap;
			return;
		});
	});
}

void ImageEx::OnApplyTemplate()
{
	Control::OnApplyTemplate();

	m_rootGrid = (Grid^)GetTemplateChild(L"RootGrid");
	m_canvasControl = (CanvasControl^)GetTemplateChild(L"CanvasControl");

	m_canvasControl->Draw += ref new TypedEventHandler<CanvasControl ^, CanvasDrawEventArgs ^>(this, &CustomControlNative::ImageEx::OnDraw);
	m_canvasControl->CreateResources += ref new Windows::Foundation::TypedEventHandler<Microsoft::Graphics::Canvas::UI::Xaml::CanvasControl ^, Microsoft::Graphics::Canvas::UI::CanvasCreateResourcesEventArgs ^>(this, &CustomControlNative::ImageEx::OnCreateResources);
}


void CustomControlNative::ImageEx::OnDraw(CanvasControl ^sender, CanvasDrawEventArgs ^args)
{
	if (m_bitmap == nullptr) return;
	auto ds = args->DrawingSession;

	auto size = sender->Size;

	CanvasImageBrush^ brush = ref new CanvasImageBrush(sender, m_bitmap);
	brush->Transform = *(ref new Windows::Foundation::Numerics::float3x2(1, 0, 0, 0, 0, 1));
	ds->FillRectangle(Rect(0, 0, 500, 500), brush);

	delete ds;
	ds = nullptr;
}


void CustomControlNative::ImageEx::OnLoaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e)
{
	int i = 0;
}


void CustomControlNative::ImageEx::OnCreateResources(Microsoft::Graphics::Canvas::UI::Xaml::CanvasControl ^sender, Microsoft::Graphics::Canvas::UI::CanvasCreateResourcesEventArgs ^args)
{
	CreateResourceAsync(sender);
}
