#!/usr/bin/env bash


CF="\033[0m"
BF="\033[1m"
RCF="\033[31m"
GCF="\033[32m"
LYCF="\033[93m"


USAGE="
${BF}Unity test results prettifier.${CF}

${BF}Usage:${CF}
  ./unity-results-prettifier.sh <${UF}path${CF}>

${BF}Parameters:${CF}
  path - Full path to test result XML which should be processed and rest 
         results printed to console.
"

if [[ $1 == "--help" ]]; then
  echo -e "$USAGE"
  exit 0
fi


#
# Generate indention prefix for result entries.
#
# Usage:
#   indention_prefix <level>
#
# Parameters:
#   level - Indention level for which prefix should be generated.
#
indention_prefix() {
  local indention=""

  for (( i = 0; i < $1; i++ )); do
    indention+="  "
  done

  echo "$indention"
}

#
# Search for file by name in specified directory and subdirectories.
#
# Usage:
#   get_xml_attribute <type> <name> <xml>
#
# Parameters:
#   type - Type of attribute which should be retrieved (string or integer).
#   name - Name of attribute which should be extracted.
#   xml  - XML node from which attribute should be extracted.
#
get_xml_attribute() {
  if [[ $1 == integer ]]; then
    [[ $3 =~ $2=\"([0-9]+)\.?.+\" ]] && echo "${BASH_REMATCH[1]}" 
  else
    [[ $3 =~ $2=\"([a-zA-Z0-9\.\:_ ,]+)\" ]] && echo "${BASH_REMATCH[1]}"
	fi
}

#
# Generate test suite entry in test results.
#
# Usage:
#   test_suite_entry <node> <indention>
#
# Parameters:
#   node      - XML node with attributes required to build suite 
#               information entry.
#   indention - Current entries indention level.
#
test_suite_entry() {
  local result="$(get_xml_attribute string result "$1")"
  local name="$(get_xml_attribute string name "$1")"
  local indention="$(indention_prefix $2)"
  local header="$indention$name"

  if [[ $result == Failed ]]; then
    header+=" ${CF}${RCF}(failed)${CF}\n"
  elif [[ $result == Skipped ]]; then
    header+=" ${CF}${LYCF}(skipped)${CF}\n"
  else
    header+="\n"
	fi

  echo "$header"
}

#
# Generate test suite results with overall information.
#
# Usage:
#   test_suite_results <node> <indention>
#
# Parameters:
#   node - XML node with attributes required to build suite information entry.
#
test_suite_results() {
  local duration="$(get_xml_attribute integer duration "$1")"
  local passed="$(get_xml_attribute integer passed "$1")"
  local failed="$(get_xml_attribute integer failed "$1")"
  local skipped="$(get_xml_attribute integer skipped "$1")"
  local result="$(get_xml_attribute string result "$1")"
  local footer=""

  if [[ $passed -gt 0 ]]; then
    footer="${CF}${GCF}Passed ${BF}$passed${CF}"
  fi

  if [[ $failed -gt 0 ]]; then
    [[ -n $footer ]] && footer+=", ${CF}${RCF}failed${CF} " || \
      footer="${CF}${RCF}Failed${CF} "
    footer+="${CF}${RCF}${BF}$failed${CF}"
  fi

  if [[ $skipped -gt 0 ]]; then
    [[ -n $footer ]] && footer+=", ${CF}${LYCF}skipped${CF} " || \
      footer="${CF}${RCF}Skipped${CF} "
    footer+="${CF}${LYCF}${BF}$skipped${CF}"
  fi

  if [[ -n $footer ]]; then
    footer+=" ($duration seconds)"
  else
	  footer="${CF}${LYCF}Empty test suite${CF}"
  fi

  echo "$footer"
}

#
# Generate test case entry in test results.
#
# Usage:
#   test_case_entry <node> <indention>
#
# Parameters:
#   node      - XML node with attributes required to build case 
#               information entry.
#   indention - Current entries indention level.
#
test_case_entry() {
  local duration="$(get_xml_attribute integer duration "$1") seconds"
  local result="$(get_xml_attribute string result "$1")"
  local name="$(get_xml_attribute string name "$1")"
  local indention="$(indention_prefix $2)"
  local entry=""

  if [[ $result == Failed ]]; then
    entry="${CF}$indention${RCF}${BF}✗${CF}${RCF} $name ($duration)${CF}\n"
  elif [[ $result == Skipped ]]; then
    entry="${CF}$indention${LYCF}${BF}○${CF} $name (skipped)${CF}\n"
  else
    entry="${CF}$indention${GCF}${BF}✓${CF} $name ($duration)\n"
  fi

  echo "$entry"
}


# Used to store list of failed test cases.
FAILED_TEST_CASES=()
# Used to store prepared test suite footer text with tests result information.
TESTS_FOOTER=""
# Current test suite / cases entries indention level.
TEST_ENTRY_INDENT_LEVEL=0
# How many root 'test-suite' elements should be skipped before starting result 
# parsing.
TEST_SUITE_TO_SKIP=2
# Variable which will store parsed information.
PARSED_RESULTS=""
# Whether searching for error message explaining test failure reason or not.
SHOULD_SEARCH_ERROR_MESSAGE=0
PARSING_ERROR_MESSAGE=0
PARSED_ERROR_MESSAGE=""


# Iterate through test report XML file line by line.
while IFS= read -r line; do
  # Skip wrapping test suites.
  if [[ $line =~ \<test-suite ]] && [[ $TEST_SUITE_TO_SKIP -gt 0 ]]; then
    let TEST_SUITE_TO_SKIP=TEST_SUITE_TO_SKIP-1
    continue
  fi

  # Check whether sub-suite information has been found for parsing or not.
  if [[ $line =~ \<test-suite ]]; then
    # Generate overall report using root test suite (first which will be found).
    if [[ -z $TESTS_FOOTER ]]; then
      TESTS_FOOTER="$(test_suite_results "$line")"
    fi

    PARSED_RESULTS+="$(test_suite_entry "$line" "$TEST_ENTRY_INDENT_LEVEL")"
    let TEST_ENTRY_INDENT_LEVEL=TEST_ENTRY_INDENT_LEVEL+1
  fi

  # Check whether exiting test suite or not.
  if [[ $line =~ \<\/test-suite\> ]] && [[ $TEST_ENTRY_INDENT_LEVEL -gt 0 ]]; then
    let TEST_ENTRY_INDENT_LEVEL=TEST_ENTRY_INDENT_LEVEL-1
  fi

  # Check whether test case information has been found for parsing or not.
  if [[ $line =~ \<test-case ]]; then
    PARSED_RESULTS+="$(test_case_entry "$line" $"TEST_ENTRY_INDENT_LEVEL")"
    TEST_CASE_FULLNAME="$(get_xml_attribute string fullname "$line")"
    TEST_CASE_RESULT="$(get_xml_attribute string result "$line")"

    if [[ $TEST_CASE_RESULT == Failed ]]; then
      FAILED_TEST_CASES+=("  ${CF}${RCF}$TEST_CASE_FULLNAME${CF}")
      SHOULD_SEARCH_ERROR_MESSAGE=1
    fi
  fi

  # Check whether searching for failed tests case error description message or not.
	if [[ $SHOULD_SEARCH_ERROR_MESSAGE == 1 ]] && [[ $line =~ \<message\> ]]; then
		[[ $line =~ \<\!\[CDATA\[[[:space:]]+(.*) ]] && \
		  PARSED_ERROR_MESSAGE+="      ${BASH_REMATCH[1]}\n"
		SHOULD_SEARCH_ERROR_MESSAGE=0
		PARSING_ERROR_MESSAGE=1
	elif [[ $PARSING_ERROR_MESSAGE == 1 ]] && [[ $line =~ \<\/message\> ]]; then
		FAILED_TEST_CASES+=("${CF}${RCF}$PARSED_ERROR_MESSAGE${CF}")
		PARSING_ERROR_MESSAGE=0
		PARSED_ERROR_MESSAGE=""
	elif [[ $PARSING_ERROR_MESSAGE == 1 ]] && ! [[ $line =~ ^$ ]]; then
		PARSED_ERROR_MESSAGE+="      $(sed -E \
	    -e 's/^[[:space:]]*//' \
    <<< $line)\n"
	fi

done <<< "$(< "$1")"	


# Print out generated tests result.
echo -e "$PARSED_RESULTS"

if [[ ${#FAILED_TEST_CASES} -gt 0 ]]; then
	echo -e "${CF}${RCF}${BF}Failed tests:${CF}\n$(IFS=$'\n'; echo "${FAILED_TEST_CASES[*]}")\n"
fi

echo -e "$TESTS_FOOTER"
