#!/usr/bin/env bash
set -euo pipefail

mgcb_dll="$1"
shift

if [[ "$(uname -s)" == "Darwin" ]]; then
  native_dir="$PWD/Content/obj/native"
  mkdir -p "$native_dir"

  freetype_lib=""
  for candidate in \
    /opt/homebrew/lib/libfreetype.6.dylib \
    /usr/local/lib/libfreetype.6.dylib \
    /opt/homebrew/lib/libfreetype.dylib \
    /usr/local/lib/libfreetype.dylib
  do
    if [[ -f "$candidate" ]]; then
      freetype_lib="$candidate"
      break
    fi
  done

  if [[ -n "$freetype_lib" ]]; then
    ln -sf "$freetype_lib" "$native_dir/libfreetype6.dylib"
    ln -sf "$freetype_lib" "$native_dir/freetype6.dylib"
    export DYLD_LIBRARY_PATH="$native_dir${DYLD_LIBRARY_PATH:+:$DYLD_LIBRARY_PATH}"
  fi
fi

exec dotnet "$mgcb_dll" "$@"
