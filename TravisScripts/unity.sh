#!/usr/bin/env bash


CF="\033[0m"
BF="\033[1m"
DF="\033[2m"
UF="\033[4m"
BRCF="\033[1;31m"
GCF="\033[32m"
LYCF="\033[93m"
LBCF="\033[94m"
LCCF="\033[96m"


USAGE="
${BF}Unity install & runner helper script.${CF}

It is ${BF}${UF}required${CF} that ${BF}${LBCF}UNITYCI_NEW_USER${CF}, ${BF}${LBCF}UNITYCI_NEW_PASS${CF} and ${BF}${LBCF}UNITYCI_NEW_SERIAL${CF} 
environment variables is set before script usage (Unity depends on these 
variables).

This script can be used to perform following actions:
  - download specified Unity package.
  - install downloaded Unity package.
  - activate / deactivate installed Unity copy.

${BF}Usage:${CF}
  ./unity.sh <${UF}action${CF}> ...
  ./unity.sh download <${UF}package-name${CF}> <${UF}package-hash${CF}> <${UF}download-path${CF}>
  ./unity.sh install <${UF}package-name${CF}> <${UF}download-path${CF}> ${DF}[install-path]${CF}
  ./unity.sh test <${UF}mode${CF}> <${UF}project${CF}> ${DF}[install-path]${CF} 
  ./unity.sh build <${UF}project${CF}> <${UF}build-path${CF}> ${DF}[install-path]${CF}
  ./unity.sh deactivate ${DF}[install-path]${CF}

${BF}Parameters:${CF}
  action        - Action which should be performed on Unity.
                  At this moment supported: '${BF}${LBCF}download${CF}', '${BF}${LBCF}install${CF}', '${BF}${LBCF}test${CF}', 
                  '${BF}${LBCF}build${CF}' and '${BF}${LBCF}deactivate${CF}'.
  package-name  - Name of Unity package which should be used for specified 
                  ${BF}${LBCF}action${CF}.
  package-hash  - Unique hash using which it is possible to download specific 
                  Unity package.
  download-path - Full path which should be used with '${BF}${LBCF}download${CF}' or '${BF}${LBCF}install${CF}' 
                  action.
  install-path  - Full path to location which should be used as installation 
                  target.
                  ${BF}Default:${CF} '${BF}${LBCF}/${CF}'
  mode          - Unity tests running mode. 
                  At this moment supported: '${BF}${LBCF}edit${CF}' and '${BF}${LBCF}play${CF}'.
  project       - Name of project which can be used to run tests or build 
                  package.
  build-path    - Full path to location where build 'unitypackage' should be 
                  stored.

${BF}Options:${CF}
  ... - Depending from ${BF}${LBCF}action${CF}, it is expected to pass values here.
"

if [[ $1 == "--help" ]]; then
  echo -e "$USAGE"
  exit 0
fi

#
# Search for file by name in specified directory and subdirectories.
#
# Usage:
#   pn_find_path <path> <name>
#
# Parameters:
#   path - Directory inside of which file should be searched.
#   name - Name of file for which path should be found.
#
pn_find_path() {
  echo "$(find "$1" -type f -iname "$2" -print -quit 2>/dev/null)"
}

if [[ -z $1 ]]; then
  echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Action name is missing.${CF}"
  exit 1
elif ! [[ $1 =~ (download|install|test|build|deactivate) ]]; then
  echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unknown action found: $1${CF}"
  exit 2
fi

unity_deactivate() {
  echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Returning Unity license place...${CF} "
  "$1" -quit -batchmode -returnlicense > /dev/null 2>&1
  local unityExitCode="$?"

  if [[ $unityExitCode -ne 0 ]]; then
    echo -e "${CF}${BRCF}failed${CF}"
    echo -e "${CF}${LYCF}[${DF}unity${CF}${LYCF}] Unity license deactivation failed." \
      "${CF}${LYCF}This machine doesn't contain activated license to return.${CF}"
  else
    echo -e "${CF}${GCF}done${CF}"
  fi
}

# Path to directory where installed Unity package can be found.
[[ $1 == deactivate ]] && INSTALLATION_PATH="$2" || INSTALLATION_PATH="$4"

[[ -z $INSTALLATION_PATH ]] && INSTALLATION_PATH="/"
[[ $INSTALLATION_PATH != "/" ]] && UNITY_SEARCH_PATH="$(dirname "$INSTALLATION_PATH")" || UNITY_SEARCH_PATH="/Applications"

if [[ $1 =~ (test|build|deactivate) ]]; then
  if [[ -z $UNITYCI_NEW_USER ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity license user is missing (UNITYCI_NEW_USER)${CF}"
    exit 3
  elif [[ -z $UNITYCI_NEW_PASS ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity license user password is missing (UNITYCI_NEW_PASS)${CF}"
    exit 4
  elif [[ -z $UNITYCI_NEW_SERIAL ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity license serial is missing (UNITYCI_NEW_SERIAL)${CF}"
    exit 5
  fi

  echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Searching installed Unity...${CF} "
  UNITY_PATH="$(pn_find_path "$UNITY_SEARCH_PATH" unity)"

  if ! [[ -r $UNITY_PATH ]]; then
    echo -e "${BRCF}not found in any subdirectories of: '${DF}$UNITY_SEARCH_PATH${CF}${BRCF}'${CF}"
    exit 6
  fi

  echo -e "${CF}${GCF}has been found: '${DF}$UNITY_PATH${CF}${GCF}'${CF}"
fi

# Check whether Unity package download URL specified or default should be used.
if [[ -z $UNITY_PACKAGE_DOWNLOAD_BASE_URL ]]; then
  UNITY_PACKAGE_DOWNLOAD_BASE_URL="https://download.unity3d.com/download_unity"
fi

[[ $1 == test ]] && UNITY_PROJECT_NAME="$3" || UNITY_PROJECT_NAME="$2"
UNITY_LOGFILE_PATH="$(pwd)/logs/unity.log"

if [[ $CI == true ]] && [[ $1 != deactivate ]] && [[ -r "$UNITY_LOGFILE_PATH" ]]; then
  echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Previous build log file has been found. Probably previous build failed. 
${BRCF}Terminating build...${CF}"
  exit 7
fi


if [[ $1 == download ]]; then
  if [[ -z $2 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity package name is missing.${CF}"
    exit 8
  elif [[ -z $3 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity package hash is missing.${CF}"
    exit 9
  elif [[ -z $4 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity package download path is missing.${CF}"
    exit 10
  fi


  UNITY_PACKAGE_DOWNLOAD_URL="$UNITY_PACKAGE_DOWNLOAD_BASE_URL/$3/$2.pkg"
  UNITY_PACKAGE_INSTALLER_PATH="$4/$(tr "/" "-" <<< "$2").pkg"

  # Create download path structure if required.
  mkdir -p "$(dirname "$UNITY_PACKAGE_INSTALLER_PATH")"

  # Retrieve Unity package size
  UNITY_PACKAGE_HEAD_RESPONSE="$(curl --head --silent "$UNITY_PACKAGE_DOWNLOAD_URL" --write-out "%{http_code}")"
  UNITY_PACKAGE_HEAD_STATUS_CODE="${UNITY_PACKAGE_HEAD_RESPONSE:${#UNITY_PACKAGE_HEAD_RESPONSE}-3:3}"
  [[ $UNITY_PACKAGE_HEAD_RESPONSE =~ Content-Length\:[[:space:]]([0-9]+) ]] && UNITY_PACKAGE_SIZE="${BASH_REMATCH[1]}"


  # Check whether package information has been fetched and match to downloaded file (if any).
  if [[ $UNITY_PACKAGE_HEAD_STATUS_CODE -ne 200 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unable to download Unity package from: ${DF}$UNITY_PACKAGE_DOWNLOAD_URL${CF}"
    exit 11
  elif [[ -r $UNITY_PACKAGE_INSTALLER_PATH ]] && ! [[ $(ls -l "$UNITY_PACKAGE_INSTALLER_PATH") =~ $UNITY_PACKAGE_SIZE ]]; then
    echo -en "${CF}${LYCF}[${DF}unity${CF}${LYCF}] '${DF}$2${CF}${LYCF}' installer already downloaded, but size doesn't match." \
      "${LYCF}Removing...${CF} "
    rm "$UNITY_PACKAGE_INSTALLER_PATH"
    echo -e "${CF}${GCF}done${CF}"
  fi

  # Check whether package should be downloaded.
  if [[ -r $UNITY_PACKAGE_INSTALLER_PATH ]]; then
    echo -e "${CF}${GCF}[${DF}unity${CF}${GCF}] '${DF}$2${CF}${GCF}' installer already downloaded to: '${DF}$4${CF}${GCF}'${CF}"
  else
    echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Downloading '${DF}$2${CF}${LCCF}' installer to: '${DF}$4${CF}${LCCF}'...${CF} "
    curl --silent --retry 5 -o "$UNITY_PACKAGE_INSTALLER_PATH" "$UNITY_PACKAGE_DOWNLOAD_URL"
    echo -e "${CF}${GCF}done${CF}"
  fi
elif [[ $1 == install ]]; then
  if [[ -z $2 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity package name is missing.${CF}"
    exit 12
  elif [[ -z $3 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity package installer location path is missing.${CF}"
    exit 13
  fi

  UNITY_PACKAGE_INSTALLER_PATH="$3/$(tr "/" "-" <<< "$2").pkg"

  if [[ -z $UNITY_PATH ]]; then
    if ! [[ -r $UNITY_PACKAGE_INSTALLER_PATH ]]; then
      echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity installer is missing: ${DF}$UNITY_PACKAGE_INSTALLER_PATH${CF}"
      exit 14
    else
      echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Installing '${DF}$2${CF}${LCCF}' to: '${DF}$INSTALLATION_PATH${CF}${LCCF}'...${CF} "

      # Try to install Unity using provided installer.
      if ! sudo installer -dumplog -package "$UNITY_PACKAGE_INSTALLER_PATH" -target "$INSTALLATION_PATH" > /dev/null 2>&1; then
        echo -e "${CF}${BRCF}failed"

        sudo installer -dumplog -package "$UNITY_PACKAGE_INSTALLER_PATH" -target "$INSTALLATION_PATH"
      fi

      echo -e "${CF}${GCF}done${CF}"
    fi
  else
    echo -e "${CF}${GCF}[${DF}unity${CF}${GCF}] '${DF}$2${CF}${GCF}' already installed to: '${DF}$UNITY_PATH${CF}${GCF}'${CF}"
  fi


  # Find installed Unity bundle and check network accessibility.
  if [[ -z $UNITY_PATH ]]; then
    echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Searching installed '${DF}$2${CF}${LCCF}'...${CF} "
    UNITY_PATH="$(pn_find_path "$UNITY_SEARCH_PATH" unity)"

    if ! [[ -r $UNITY_PATH ]]; then
      echo -e "${BRCF}not found in any subdirectories of: '${DF}$UNITY_SEARCH_PATH${CF}${BRCF}'${CF}"
      exit 15
    fi

    echo -e "${CF}${GCF}has been found: '${DF}$UNITY_PATH${CF}${GCF}'${CF}"
  fi
  

  SOCKET_FILTER_PATH="/usr/libexec/ApplicationFirewall/socketfilterfw"

  if ! [[ -r $SOCKET_FILTER_PATH ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] '${DF}socketfilterfw${CF}${BRCF}' not found.${CF}"
    exit 16
  fi

  echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Checking '${DF}$UNITY_PATH${CF}${LCCF}' network accessibility...${CF} "

  if ! "$SOCKET_FILTER_PATH" --getappblocked "$UNITY_PATH" > /dev/null 2>&1; then
    echo -e "${CF}${BRCF}failed"

    "$SOCKET_FILTER_PATH" --getappblocked "$UNITY_PATH"
  fi

  echo -e "${CF}${GCF}done${CF}"

  # Copy certificates.
  echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Copying '${DF}CACerts.pem${CF}${LCCF}' to: '${DF}$HOME/Library/Unity/Certificates/${CF}${GCF}'...${CF} "
  mkdir -p "$HOME/Library/Unity/Certificates"
  cp "./TravisScripts/CACerts.pem" "$HOME/Library/Unity/Certificates/"
  echo -e "${CF}${GCF}done${CF}"
elif [[ $1 == test ]]; then
  if [[ -z $2 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Test mode is missing (expected: edit or play).${CF}"
    exit 17
  elif [[ -z $3 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity project name is missing.${CF}"
    exit 18
  fi
  
  UNITY_TEST_PLATFORM="${2}mode"
  UNITY_TEST_RESULTS_PATH="$(pwd)/${UNITY_TEST_PLATFORM}-test-results.xml"
  echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Running editor unit tests in '${DF}$UNITY_TEST_PLATFORM${CF}${LCCF}'...${CF} "
  
  if [[ $2 == edit ]]; then
    "$UNITY_PATH" \
      -batchmode \
      -logFile "$UNITY_LOGFILE_PATH" \
      -projectPath "$(pwd)/$UNITY_PROJECT_NAME" \
      -runEditorTests \
      -editorTestsResultFile "$UNITY_TEST_RESULTS_PATH" \
      -testPlatform "$UNITY_TEST_PLATFORM" \
      -username "$UNITYCI_NEW_USER" \
      -password "$UNITYCI_NEW_PASS" \
      -serial "$UNITYCI_NEW_SERIAL" > /dev/null 2>&1
  else
    "$UNITY_PATH" \
      -batchmode \
      -logFile "$UNITY_LOGFILE_PATH" \
      -projectPath "$(pwd)/$UNITY_PROJECT_NAME" \
      -runEditorTests \
      -testResults "$UNITY_TEST_RESULTS_PATH" \
      -testPlatform "$UNITY_TEST_PLATFORM" \
      -username "$UNITYCI_NEW_USER" \
      -password "$UNITYCI_NEW_PASS" \
      -serial "$UNITYCI_NEW_SERIAL" > /dev/null 2>&1
  fi

  UNITY_EXIT_CODE="$?"

  if [[ $UNITY_EXIT_CODE == 0 ]]; then
    echo -e "${CF}${GCF}done${CF}"
  else
    echo -e "${CF}${BRCF}failed${CF}"
  fi

  # Print test results if possible.
  if [[ -r $UNITY_TEST_RESULTS_PATH ]]; then
    . "$(dirname "$(pwd)/$0")/unity-results-prettifier.sh" "$UNITY_TEST_RESULTS_PATH"
  else
    # Deactivate license just in case (Travis build may error and failure/success 
    # handlers with deactivation won't be called).
    unity_deactivate "$UNITY_PATH"

    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Tests report not found: ${DF}$UNITY_TEST_RESULTS_PATH${CF}"
    
    if [[ -r "$UNITY_LOGFILE_PATH" ]]; then
      echo -e "${CF}${BRCF}[${DF}unity${CF}${BRCF}] Unity tests run error (exit code: $UNITY_EXIT_CODE):\n${BRCF}$(< "$UNITY_LOGFILE_PATH" head -n 50)"
    fi

    sleep 2
    exit 19
  fi

  # Deactivate license just in case (Travis build may error and failure/success 
  # handlers with deactivation won't be called).
  unity_deactivate "$UNITY_PATH"

  # Check whether script should exit with Unity provided error code.
  if [[ $UNITY_EXIT_CODE -ne 0 ]]; then
    sleep 2
    exit $UNITY_EXIT_CODE
  else
    # Remove build log file (used to indicate whether previous test run / build was successful on CI).
    if [[ -r "$UNITY_LOGFILE_PATH" ]]; then
      echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Removing build log '${DF}$UNITY_LOGFILE_PATH${CF}${LCCF}'...${CF} "
      rm "$UNITY_LOGFILE_PATH"
      echo -e "${CF}${GCF}done${CF}"
    fi
  fi
elif [[ $1 == build ]]; then
  if [[ -z $2 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity project name is missing.${CF}"
    exit 20
  elif [[ -z $3 ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity package build path is missing.${CF}"
    exit 21
  elif ! [[ $3 =~ .*unitypackage$ ]]; then
    echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Unity package build path should include package file name.${CF}"
    exit 22
  fi

  # Created build directory if it is missing.
  ! [[ -d "$(dirname "$3")" ]] && mkdir -p "$(dirname "$3")"

  echo -en "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Building Unity package...${CF} "

  "$UNITY_PATH" \
    -quit \
    -batchmode \
    -logFile "$UNITY_LOGFILE_PATH" \
    -projectPath "$(pwd)/$UNITY_PROJECT_NAME" \
    -exportPackage "Assets" "$(basename "$3")" \
    -username "$UNITYCI_NEW_USER" \
    -password "$UNITYCI_NEW_PASS" \
    -serial "$UNITYCI_NEW_SERIAL" > /dev/null 2>&1
  UNITY_EXIT_CODE="$?"
    
  # Deactivate license just in case (Travis build may error and failure/success 
  # handlers with deactivation won't be called).
  unity_deactivate "$UNITY_PATH"

  # Check whether script should exit with Unity provided error code.
  if [[ $UNITY_EXIT_CODE -ne 0 ]]; then
    echo -e "${CF}${BRCF}failed${CF}"
    
    if [[ -r "$UNITY_LOGFILE_PATH" ]]; then
      echo -e "${CF}${BRCF}[${DF}unity${CF}${BRCF}] Package export error:\n${BRCF}$(< "$UNITY_LOGFILE_PATH" head -n 50)${CF}"
    fi

    sleep 2
    exit $UNITY_EXIT_CODE
  else
    # Check whether package has been built.
    if ! [[ -r "$(pwd)/$2/$(basename "$3")" ]]; then
      echo -e "${BRCF}[${DF}unity${CF}${BRCF}] Built package not found: ${DF}$(pwd)/$2/$(basename "$3")${CF}"
      exit 23
    else
      cp "$(pwd)/$2/$(basename "$3")" "$3"
    fi
    
    echo -e "${CF}${GCF}done${CF}"

    # Remove build log file (used to indicate whether previous test run / build was successful on CI).
    if [[ -r "$UNITY_LOGFILE_PATH" ]]; then
      echo -e "${CF}${LCCF}[${DF}unity${CF}${LCCF}] Removing build log '${DF}$UNITY_LOGFILE_PATH${CF}${LCCF}'...${CF} "
      rm "$UNITY_LOGFILE_PATH"
      echo -e "${CF}${GCF}done${CF}"
    fi
  fi
elif [[ $1 == deactivate ]]; then
  unity_deactivate "$UNITY_PATH"
fi
