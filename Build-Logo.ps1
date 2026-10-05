[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

if (-not (Get-Command magick -ErrorAction SilentlyContinue)) {
    throw 'ImageMagick is required. Install it with winget install ImageMagick.ImageMagick.'
}

$sourceDirectory = Join-Path $PSScriptRoot 'assets'
$temporaryPath = Join-Path $sourceDirectory ('.logo-' + [System.Guid]::NewGuid().ToString('N'))
$iconSizes = 512, 256, 128, 64, 48, 32, 16

$backgroundColor = '#172033'
# Preblended equivalent of #0F1626 at 70% over the background. The source
# background has a shadow-shaped knockout, so the generated facet must be opaque.
$shadowColor = '#11192A'
$hourglassColor = '#D6A84A'

function Invoke-ImageMagick {
    param([Parameter(Mandatory)][string[]] $Arguments)

    & magick @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "ImageMagick failed with exit code $LASTEXITCODE."
    }
}

function New-ColoredLayer {
    param(
        [Parameter(Mandatory)][string] $Source,
        [Parameter(Mandatory)][string] $Color,
        [Parameter(Mandatory)][string] $Destination
    )

    Invoke-ImageMagick @(
        $Source,
        '-channel', 'RGB',
        '-fill', $Color,
        '-colorize', '100',
        '+channel',
        $Destination
    )
}

function New-Composition {
    param(
        [Parameter(Mandatory)][string[]] $Layers,
        [Parameter(Mandatory)][string] $Destination
    )

    $flattened = Join-Path $temporaryPath 'flattened.png'
    $trimmed = Join-Path $temporaryPath 'trimmed.png'

    Invoke-ImageMagick @(
        $Layers
        '-background', 'none'
        '-layers', 'flatten'
        $flattened
    )
    Invoke-ImageMagick @($flattened, '-trim', '+repage', $trimmed)

    $dimensions = (& magick identify -format '%w %h' $trimmed) -split ' '
    if ($LASTEXITCODE -ne 0 -or $dimensions.Count -ne 2) {
        throw 'ImageMagick could not determine the composition dimensions.'
    }

    $contentSize = [Math]::Max([int] $dimensions[0], [int] $dimensions[1])
    $padding = [Math]::Ceiling($contentSize * 0.075)
    $canvasSize = $contentSize + (2 * $padding)

    Invoke-ImageMagick @(
        $trimmed,
        '-background', 'none',
        '-bordercolor', 'none',
        '-border', "${padding}x${padding}",
        '-gravity', 'center',
        '-extent', "${canvasSize}x${canvasSize}",
        $Destination
    )
}

function New-ResizedPng {
    param(
        [Parameter(Mandatory)][string] $Source,
        [Parameter(Mandatory)][int] $Size,
        [Parameter(Mandatory)][string] $Destination
    )

    Invoke-ImageMagick @(
        $Source,
        '-filter', 'Lanczos',
        '-resize', "${Size}x${Size}",
        '-strip',
        '-define', 'png:exclude-chunk=date,time',
        $Destination
    )
}

New-Item -ItemType Directory -Path $temporaryPath | Out-Null

try {
    $background = Join-Path $temporaryPath 'background.png'
    $shadow = Join-Path $temporaryPath 'shadow.png'
    $hourglassLeft = Join-Path $temporaryPath 'hourglass-left.png'
    $hourglassMiddle = Join-Path $temporaryPath 'hourglass-middle.png'
    $hourglassRight = Join-Path $temporaryPath 'hourglass-right.png'
    $composition = Join-Path $temporaryPath 'composition.png'

    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-light.png') `
        -Color $backgroundColor `
        -Destination $background
    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-shadow.png') `
        -Color $shadowColor `
        -Destination $shadow
    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-left.png') `
        -Color $hourglassColor `
        -Destination $hourglassLeft
    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-middle.png') `
        -Color $backgroundColor `
        -Destination $hourglassMiddle
    New-ColoredLayer `
        -Source (Join-Path $sourceDirectory 'chrononuensis-hourglass-right.png') `
        -Color $hourglassColor `
        -Destination $hourglassRight

    New-Composition `
        -Layers @($background, $shadow, $hourglassMiddle, $hourglassLeft, $hourglassRight) `
        -Destination $composition

    $resized = @{}
    foreach ($size in $iconSizes) {
        $destination = if ($size -in 512, 256, 128, 64) {
            Join-Path $sourceDirectory "chrononuensis-icon-$size.png"
        }
        else {
            Join-Path $temporaryPath "chrononuensis-icon-$size.png"
        }

        New-ResizedPng -Source $composition -Size $size -Destination $destination
        $resized[$size] = $destination
    }

    Copy-Item -LiteralPath $resized[128] -Destination (Join-Path $PSScriptRoot 'chrononuensis-icon-128.png') -Force
    Copy-Item -LiteralPath $resized[64] -Destination (Join-Path $sourceDirectory 'chrononuensis-favicon.png') -Force
    Copy-Item -LiteralPath $resized[64] -Destination (Join-Path $PSScriptRoot 'docs/uploads/chrononuensis-icon-64.png') -Force
    Copy-Item -LiteralPath $resized[256] -Destination (Join-Path $PSScriptRoot 'docs/uploads/chrononuensis-icon-256.png') -Force
    Copy-Item -LiteralPath $resized[64] -Destination (Join-Path $PSScriptRoot 'docs/uploads/favicon.png') -Force

    $iconDestination = Join-Path $sourceDirectory 'chrononuensis.ico'
    Invoke-ImageMagick @(
        $resized[256],
        $resized[128],
        $resized[64],
        $resized[48],
        $resized[32],
        $resized[16],
        $iconDestination
    )

    Copy-Item -LiteralPath $iconDestination -Destination (Join-Path $sourceDirectory 'favicon.ico') -Force
    Copy-Item -LiteralPath $iconDestination -Destination (Join-Path $PSScriptRoot 'docs/uploads/favicon.ico') -Force
}
finally {
    $resolvedTemporaryPath = [System.IO.Path]::GetFullPath($temporaryPath)
    $resolvedSourceDirectory = [System.IO.Path]::GetFullPath($sourceDirectory) + [System.IO.Path]::DirectorySeparatorChar

    if ($resolvedTemporaryPath.StartsWith($resolvedSourceDirectory, [System.StringComparison]::OrdinalIgnoreCase)) {
        Remove-Item -LiteralPath $resolvedTemporaryPath -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Output $sourceDirectory
