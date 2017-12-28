using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace gymlocator.Controls
{

    public delegate void RepeaterViewItemAddedEventHandler(object sender, RepeaterViewItemAddedEventArgs args);

    // in lieu of an actual Xamarin Forms ItemsControl, this is a heavily modified version of code from https://forums.xamarin.com/discussion/21635/xforms-needs-an-itemscontrol
    public class RepeaterView : StackLayout
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            "ItemSource",
            typeof(IEnumerable),
            typeof(RepeaterView),
            new List<object>(),
            BindingMode.OneWay,
            propertyChanged: ItemsChanged);

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(RepeaterView),
            default(DataTemplate));

        public event RepeaterViewItemAddedEventHandler ItemCreated;

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (RepeaterView)bindable;
                var oldObservableCollection = oldValue as INotifyCollectionChanged;

                if (oldObservableCollection != null)
                {
                    oldObservableCollection.CollectionChanged -= control.OnItemsSourceCollectionChanged;
                }

                var newObservableCollection = newValue as INotifyCollectionChanged;

                if (newObservableCollection != null)
                {
                    newObservableCollection.CollectionChanged += control.OnItemsSourceCollectionChanged;
                }

                control.Children.Clear();

                if (newValue != null)
                {
                    foreach (var item in (IEnumerable)newValue)
                    {
                        var view = control.CreateChildViewFor(item);
                        control.Children.Add(view);
                        control.OnItemCreated(view);
                    }
                }

                control.UpdateChildrenLayout();
                control.InvalidateLayout();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        protected virtual void OnItemCreated(View view) =>
        this.ItemCreated?.Invoke(this, new RepeaterViewItemAddedEventArgs(view, view.BindingContext));

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var invalidate = false;

            if (e.OldItems != null)
            {
                this.Children.RemoveAt(e.OldStartingIndex);
                invalidate = true;
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; ++i)
                {
                    var item = e.NewItems[i];
                    var view = this.CreateChildViewFor(item);

                    this.Children.Insert(i + e.NewStartingIndex, view);
                    OnItemCreated(view);
                }

                invalidate = true;
            }

            if (invalidate)
            {
                this.UpdateChildrenLayout();
                this.InvalidateLayout();
            }
        }

        private View CreateChildViewFor(object item)
        {
            this.ItemTemplate.SetValue(BindableObject.BindingContextProperty, item);
            return (View)this.ItemTemplate.CreateContent();
        }
    }

    public class RepeaterViewItemAddedEventArgs : EventArgs
    {
        private readonly View view;
        private readonly object model;

        public RepeaterViewItemAddedEventArgs(View view, object model)
        {
            this.view = view;
            this.model = model;
        }

        public View View => this.view;

        public object Model => this.model;
    }
}
