﻿
// -------------------------------------------
// F# Plotter
// -------------------------------------------
// Plots stock price and moving average
// (C) Johan Astborg, 2012 - joastbg@gmail.com

/// Normal distribution

#r @"C:\Users\Niklas\Documents\Visual Studio 2012\Projects\Chapter5\packages\MathNet.Numerics.2.6.1\lib\net40\MathNet.Numerics.dll"
#r "System.Windows.Forms.DataVisualization.dll"

open System
open System.Net
open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting
open Microsoft.FSharp.Control.WebExtensions
open MathNet.Numerics.Distributions;

let normd = new Normal(0.0, 1.0)

// Create chart and form
let chart = new Chart(Dock = DockStyle.Fill)
let area = new ChartArea("Main")
chart.ChartAreas.Add(area)

let mainForm = new Form(Visible = true, TopMost = true, 
                        Width = 700, Height = 500)

do mainForm.Text <- "VIX-index 2000-01-01 to 2013-11-01"
mainForm.Controls.Add(chart)

// Create serie for stock price
let stockPrice = new Series("stockPrice")
do stockPrice.ChartType <- SeriesChartType.Line
do stockPrice.BorderWidth <- 2
do stockPrice.Color <- Drawing.Color.Red
chart.Series.Add(stockPrice)

// Create serie for moving average
let movingAvg = new Series("movingAvg")
do movingAvg.ChartType <- SeriesChartType.Line
do movingAvg.BorderWidth <- 2
do movingAvg.Color <- Drawing.Color.Blue
chart.Series.Add(movingAvg)

// Syncronous fetching (just one stock here)
let fetchOne() =
    let uri = new System.Uri("http://ichart.finance.yahoo.com/table.csv?s=%5EVIX&a=00&b=1&c=2000&d=10&e=1&f=2013&g=d&ignore=.csv")
    let client = new WebClient()
    let html = client.DownloadString(uri)
    html

// Parse CSV
let getPrices() =
    let data = fetchOne()
    data.Split('\n')
    |> Seq.skip 1
    |> Seq.map (fun s -> s.Split(','))
    |> Seq.map (fun s -> float s.[4])
    |> Seq.truncate 2500

// Calc moving average
let movingAverage n (prices:seq<float>) =
    prices    
    |> Seq.windowed n
    |> Seq.map Array.sum
    |> Seq.map (fun a -> a / float n)    

// The plotting
let sp = getPrices()
do sp |> Seq.iter (stockPrice.Points.Add >> ignore)

let ma = movingAverage 100 sp
do ma |> Seq.iter (movingAvg.Points.Add >> ignore)