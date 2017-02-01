using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using OxyPlot.Xamarin.Android;

using RunningApp.Views;
using RunningApp.Exceptions;
using RunningApp.Adapters;
using RunningApp.Database;
using RunningApp.Parcels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RunningApp.Tracker;

namespace RunningApp.Fragments
{
    public class AnalyseFragment : BaseFragment
    {
        private const int DISTANCETRAVELDGRAPH = Resource.Id.rbDistanceTraveledGraph;
        private const int SPEEDGRAPH = Resource.Id.rbSpeedGraph;

        PlotView Graph;

        public AnalyseFragment() : base()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.Analyse, container, false);

            Bundle b = this.Arguments;
            TrackParcel tp = (TrackParcel)b.GetParcelable("Track");

            RadioGroup rgGraph = view.FindViewById<RadioGroup>(Resource.Id.rgGraph);
            rgGraph.CheckedChange += this.UpdateGraphType;

            this.Graph = view.FindViewById<PlotView>(Resource.Id.plot_view);

            return view;
        }

        public override void OnStart()
        {
            base.OnStart();

            this.SetGraphType(AnalyseFragment.SPEEDGRAPH);
        }

        private void SetGraphType(int type)
        {
            Bundle b = this.Arguments;
            TrackParcel tp = (TrackParcel)b.GetParcelable("Track");

            switch (type)
            {
                case Resource.Id.rbDistanceTraveledGraph:
                    this.Graph.Model = this.CreateDistanceTraveledModel(tp.Track);
                    break;
                case Resource.Id.rbSpeedGraph:
                default:
                    this.Graph.Model = this.CreateSpeedModel(tp.Track);
                    break;
            }

            this.View.FindViewById<RadioButton>(type).Checked = true;

            this.Graph.Invalidate();
        }

        private void UpdateGraphType(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            this.SetGraphType(e.CheckedId);
        }

        private PlotModel CreateDistanceTraveledModel(Track Track)
        {
            PlotModel PlotModel = new PlotModel
            {
                Title = "Afgelegde weg"
            };

            PlotModel.Axes.Add(new TimeSpanAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Tijd (s)",
                AbsoluteMinimum = TimeSpanAxis.ToDouble(new TimeSpan()),
                AbsoluteMaximum = TimeSpanAxis.ToDouble(Track.GetTotalTimeSpan())
            });
            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Afgelegde weg (m)",
                AbsoluteMinimum = 0,
                AbsoluteMaximum = Track.GetTotalRunningDistance()
            });

            PlotModel.Series.Add(new LineSeries
            {
                Title = "Afgelegde weg",
                ItemsSource = Track.GetDistanceTraveledDataPoints()
            });

            return PlotModel;
        }

        private PlotModel CreateSpeedModel(Track Track)
        {
            PlotModel PlotModel = new PlotModel
            {
                Title = "Snelheid"
            };

            PlotModel.Axes.Add(new TimeSpanAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Tijd (s)",
                AbsoluteMinimum = TimeSpanAxis.ToDouble(new TimeSpan()),
                AbsoluteMaximum = TimeSpanAxis.ToDouble(Track.GetTotalTimeSpan())
            });
            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Snelheid (m/s)",
                AbsoluteMinimum = 0,
                AbsoluteMaximum = Track.GetMaxSpeed()
            });

            PlotModel.Series.Add(new LineSeries
            {
                Title = "Snelheid",
                ItemsSource = Track.GetSpeedDataPoints(),
            });

            int i = 1;
            foreach (List<DataPoint> ldp in Track.GetSegmentAvarageSpeedDataPoints())
            {
                PlotModel.Series.Add(new LineSeries
                {
                    Title = $"Gemiddelde snelheid in segment {i}",
                    ItemsSource = ldp
                });
                i++;
            }

            PlotModel.Series.Add(new LineSeries
            {
                Title = "Gemiddelde snelheid",
                ItemsSource = Track.GetAvergageSpeedDataPoints()
            });

            return PlotModel;
        }
    }
}